using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using PhotonHashtable = ExitGames.Client.Photon;

namespace Game {
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviourPun {
        public int PlayerId;

        [SerializeField] LayerMask _interactableLayer;
        [SerializeField] Board _playerBoard;
        [SerializeField] Transform _playerPivot;

        public float stunDuration = 1f;     // should be less than punch cooldown
        public CountdownTimer StunnedTimer; // possibly replace with ref to stunned animation... or state machine?

        public bool IsDead { get; private set; }

        PlayerMovement _playerMovement;
        PlayerHealth _playerHealth;
        Camera _mainCamera;

        [Header("Pieces")]
    
        [SerializeField] Transform _heldPieceContainer;
        [SerializeField] HeldPieceOutliner _heldPieceOutliner;
        [SerializeField] PieceRenderer _ghostPr;
        Piece _heldPiece;
        List<GameObject> _nearObjs = new();

        [Header("Punching")]

        [SerializeField] float _punchCooldown = 2f;
        public int PunchDmg => _punchDmg;
        [SerializeField] int _punchDmg;
        GameObject _punchHitboxObj;
        CountdownTimer _punchCooldownTimer;

        [Header("Spells")]
    
        [SerializeField] SpellProjectileData _preparedSpellProjectile;

        public CountdownTimer SpellUseableTimer { get; private set; }
        float _wallSpellScale;

        [Header("Shield")]
        Transform _shieldTransform;
        bool _isShielding;

        [Header("Wall")]
        [SerializeField] GameObject _wallPrefab;

        void Awake() {
            _mainCamera = Camera.main;

            _playerMovement = GetComponent<PlayerMovement>();
            _playerHealth = GetComponent<PlayerHealth>();
        
            _punchHitboxObj = _playerPivot.transform.GetChild(0).gameObject;

            _shieldTransform = _playerPivot.transform.GetChild(1);
            _fullShieldScale = _shieldTransform.localScale.x;

            _punchCooldownTimer = new CountdownTimer(_punchCooldown);
            SpellUseableTimer = new CountdownTimer(Constants.SpellDuration);
            StunnedTimer = new CountdownTimer(stunDuration);
        }

        void Start() {
            _playerHealth.OnUpdateShield += ScaleShield;
            _playerHealth.OnLoseHp += DropPiece;
        }

        void Update() {
            TurnToCursor();
            HandleGhostPiece();
        }

        void TurnToCursor() {
            if (!photonView.IsMine) return;
        
            Vector3 dir = GetMouseDir().normalized;

            // Calculate the angle in degrees between the player and the mouse, rotate to face mouse
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _playerPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        [PunRPC]
        public void Death() {
            Debug.Log("Player died");
            IsDead = true;
            _playerBoard.GetComponent<BoardRenderer>().ChangeOpacity(0.5f);
            DropPiece();
            
            // TODO: add player death animation
            gameObject.SetActive(false);
        }

        #region Piece

        public bool Interact() {
            if (_heldPiece == null) {
                return PickUpPiece();
            } else {
                return PlacePiece();
            }
        }

        public bool RotatePiece() {
            if (_heldPiece) {
                _heldPiece.photonView.RPC(nameof(Piece.RotateCW), RpcTarget.All);
                _lastHoverPoint = new Vector2Int(-1, -1); // reset value for HandleGhostPiece
                return true;
            }

            return false;
        }

        bool PickUpPiece() {
            if (_nearObjs.Count == 0) return false;

            // Find nearest piece
            float minDistance = int.MaxValue;
            foreach (GameObject obj in _nearObjs.ToList()) {
                float d = Vector2.Distance(transform.position, obj.transform.position);
                if (d < minDistance && obj.TryGetComponent(out Piece piece) && obj.transform.parent == null) {
                    minDistance = d;
                    _heldPiece = piece;
                }
            }

            if (!_heldPiece) return false;

            // Pickup piece
            // Disable held piece's self movement
            if (_heldPiece.TryGetComponent(out MoveToPoint mtp)) {
                _heldPiece.photonView.RPC(nameof(MoveToPoint.Disable), RpcTarget.All);
            }

            // Activate ghost piece overlay
            _ghostPr.Init(_heldPiece);
            
            // Activate held block highlight effect
            _heldPieceOutliner.RefreshOutline(_heldPiece);

            // Take ownership of held piece
            // _heldPiece.photonView.RequestOwnership(); // server authoritative - requires Piece Ownership -> Request
            _heldPiece.photonView.TransferOwnership(photonView.Owner); // client authoritative - requires Piece Ownership -> Takeover

            // Make piece a child object of player
            NetworkManager.Instance.photonView.RPC(nameof(NetworkUtils.S_SetTransform), RpcTarget.All, _heldPiece.photonView.ViewID,
                _heldPieceContainer.localPosition, Quaternion.identity, photonView.ViewID, false);
        
            return true;
        }

        bool PlacePiece() {
            if (_heldPiece == null) return false;

            Vector2Int hoverPoint = GetHeldPiececHoverPoint();

            if (_playerBoard.PlacePiece(_heldPiece, hoverPoint.x, hoverPoint.y)) {
                // Deactivate held block highlight effect
                _heldPieceOutliner.RefreshOutline(null);
                    
                _heldPiece = null;
                return true;
            }

            // Check if trashing
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 100.0f, _interactableLayer);
            if (hit) {
                if (hit.collider.TryGetComponent(out Trash trash)) {
                    // Deactivate held block highlight effect
                    _heldPieceOutliner.RefreshOutline(null);
                    
                    trash.TrashObj(_heldPiece.gameObject);
                    _heldPiece = null;
                    return true;
                }
            }

            Debug.Log($"Unable to place block at ({hoverPoint.x}, {hoverPoint.y})");
            return false;
        }

        // Drop releases held piece at player's position, ready to be picked up again
        void DropPiece() {
            if (_heldPiece == null) return;
            
            // Deactivate held block highlight effect
            _heldPieceOutliner.RefreshOutline(null);
        
            // Release ownership of held piece
            _heldPiece.photonView.TransferOwnership(PhotonNetwork.MasterClient); // client authoritative - requires Piece Ownership -> Takeover

            // Unparent held piece
            NetworkManager.Instance.photonView.RPC(nameof(NetworkUtils.S_UnsetParent), RpcTarget.All, _heldPiece.photonView.ViewID);
            _heldPiece = null;
        }

        Vector2Int _lastHoverPoint = new Vector2Int(-1, -1);
        void HandleGhostPiece() {
            if (_heldPiece == null) {
                _ghostPr.gameObject.SetActive(false);
                return;
            }

            // Use piece's location as hover point
            Vector2Int hoverPoint = GetHeldPiececHoverPoint();
            if (_lastHoverPoint == hoverPoint) return;
            _lastHoverPoint = hoverPoint;

            // Disable ghost piece if held piece is not over player's board and vice versa
            if (_playerBoard.ValidatePiece(_heldPiece, hoverPoint.x, hoverPoint.y, _playerBoard.IsInBounds)) {
                _ghostPr.gameObject.SetActive(true);
                _heldPiece.SetEnableRender(false);
            } else {
                _ghostPr.gameObject.SetActive(false);
                _heldPiece.SetEnableRender(true);
                return;
            }

            // Move ghost piece object to grid snap pos
            _ghostPr.transform.localPosition = new Vector3(hoverPoint.x, hoverPoint.y, 0);

            // Change ghost piece visual when placement would overlap an existing piece
            // TODO: Add texture on ghost piece if invalid placement
            if (_playerBoard.ValidatePiece(_heldPiece, hoverPoint.x, hoverPoint.y, _playerBoard.IsValidPlacement)) {
                _ghostPr.RemoveBlockOverlay();
            } else {
                _ghostPr.SetBlockOverlay();
            }
        }

        void OnTriggerEnter2D(Collider2D col) {
            if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Pickup]) && !_nearObjs.Contains(col.gameObject)) {
                _nearObjs.Add(col.gameObject);
            }
        }
        void OnTriggerExit2D(Collider2D col) {
            if (_nearObjs.Contains(col.gameObject)) {
                _nearObjs.Remove(col.gameObject);
            }
        }

        #endregion

        #region Punching

        public void Punch() {
            if (_punchCooldownTimer.IsTicking) return;
            _punchCooldownTimer.Start();

            // Short dash towards mouse
            Vector3 dir = GetMouseDir().normalized;
            _playerMovement.SetVelocity(dir * (_playerMovement.MaxSpeed/2f + Constants.MinPunchDashDistance));
        
            // Do punch, which can make a hit player drop their held piece
            photonView.RPC(nameof(S_ActivatePunchHitbox), RpcTarget.All);
        }
        [PunRPC]
        public void S_ActivatePunchHitbox() { _punchHitboxObj.SetActive(true); }

        #endregion

        #region Spells

        public void Cast() {
            if (_isShielding) return;

            Vector3 dir = GetMouseDir().normalized;

            if (_wallSpellScale > 0) { // cast wall spell this Cast, if available
                CreateWall(dir, _wallSpellScale);
                _wallSpellScale = 0;
                return;
            }

            if (!SpellUseableTimer.IsTicking) return;

            _preparedSpellProjectile.MoveDir = dir;
            Vector2 startPos = dir * Constants.SpellInstantiatePosOffset + transform.position; // constant for spell spawn position offset

            Factory.Instance.photonView.RPC(nameof(Factory.S_CreateSpellObj), RpcTarget.AllViaServer, _preparedSpellProjectile, startPos);
        }

        public bool PrepareSpell() {
            Vector2Int hoverPoint = GetHoverPoint();
            Block hoverBlock = _playerBoard.SelectPosition(hoverPoint.x, hoverPoint.y);
            if (hoverBlock == null || !hoverBlock.IsActive) return false; // hovering over nothing

            List<Block> spellBlocks = _playerBoard.FindColorBlockGroup(hoverPoint.x, hoverPoint.y);
            int power = spellBlocks.Count;
            print($"Spell Power: {power}");

            // TODO: delayed prepare with animation for "charging up"

            // Consume blocks part of spell
            _playerBoard.photonView.RPC(nameof(Board.S_DisableBlocks), RpcTarget.All, (object) spellBlocks.ToArray());

            // Execute spell
            // note: replace this with more robust system if expanding spell types
            SpellType spellType = spellBlocks[0].SpellType;
            switch (spellType) {
                case SpellType.Damage:
                    _preparedSpellProjectile = new() {Dmg = power, Speed = power / 2 + 1};
                    SpellUseableTimer.Start();
                    break;
                case SpellType.Shield:
                    _playerHealth.ModifyShield(power);
                    break;
                case SpellType.Speed:
                    _playerMovement.MultiplySpeedForDuration(1f + (float) power / 30);
                    break;
                case SpellType.Wall:
                    _wallSpellScale = 1 + power / 3f;
                    break;
                default:
                    Debug.LogError($"Unable to execute spell: Unexpected SpellType {spellType.ToString()}");
                    return false;
            }

            return true;
        }

        #endregion

        #region Shield

        float _fullShieldScale;
        void ScaleShield(float shieldPercent) {
            float size = shieldPercent * _fullShieldScale;
            _shieldTransform.localScale = new Vector3(size, _shieldTransform.localScale.y, _shieldTransform.localScale.z);
        }

        public void ActivateShield() {
            _isShielding = true;
            photonView.RPC(nameof(S_ActivateShield), RpcTarget.All);
        }
        public void DeactivateShield() {
            _isShielding = false;
            photonView.RPC(nameof(S_DeactivateShield), RpcTarget.All);
        }
        [PunRPC]
        public void S_ActivateShield() { _shieldTransform.gameObject.SetActive(true); }
        [PunRPC]
        public void S_DeactivateShield() { _shieldTransform.gameObject.SetActive(false); }

        #endregion

        #region Wall

        public void CreateWall(Vector3 dir, float scaleFactor) {
            Vector3 pos = transform.position + (dir * Constants.SpellInstantiatePosOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // Rotate wall to extend away from player
            Vector3 rot = new Vector3(0, 0, angle);
            photonView.RPC(nameof(S_CreateWall), RpcTarget.All, pos, rot, scaleFactor);
        }

        [PunRPC]
        public void S_CreateWall(Vector3 position, Vector3 rotation, float scaleFactor) {
            // Create wall base object
            Transform wallPivotObj = Instantiate(_wallPrefab, position, Quaternion.Euler(rotation)).transform;

            // Scale wall outwards, keeping close edge in same place
            Transform wallObj = wallPivotObj.GetChild(0);                                             // should be wall's collider/sprite object
            float newPositionX = wallObj.localScale.x / 2.0f * scaleFactor + wallObj.localPosition.x; // calculation for obj offset after scale
            wallObj.localPosition = new Vector3(newPositionX, wallObj.localPosition.y, wallObj.localPosition.z);
            wallObj.localScale = new Vector3(wallObj.localScale.x * scaleFactor, wallObj.localScale.y, wallObj.localScale.z);
        }

        #endregion

        #region Helper

        public void Enable() {
            _playerBoard.gameObject.SetActive(true);
            enabled = true;
        }
        public void Disable() {
            _playerBoard.gameObject.SetActive(false);
            enabled = false;
        }

        Vector2Int GetHoverPoint() {
            // localPosition works when Player is child of Board and centered at origin
            return new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        }
        Vector2Int GetHeldPiececHoverPoint() {
            // localPosition works when Player is child of Board and centered at origin
            return new Vector2Int(Mathf.RoundToInt(transform.localPosition.x + _heldPieceContainer.localPosition.x),
                Mathf.RoundToInt(transform.localPosition.y + _heldPieceContainer.localPosition.y));
        }

        // Returns vector from player to mouse
        Vector3 GetMouseDir() {
            Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPosition.z = transform.position.z;
            return mouseWorldPosition - transform.position;
        }

        #endregion
    }
}