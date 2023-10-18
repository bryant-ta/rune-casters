using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using PhotonHashtable = ExitGames.Client.Photon;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviourPun {
    public int PlayerId;

    [SerializeField] LayerMask _interactableLayer;
    [SerializeField] Board _playerBoard;
    Camera _mainCamera;

    PlayerMovement _playerMovement;
    PlayerHealth _playerHealth;

    [Header("Pieces")] 
    
    Piece _heldPiece;
    List<GameObject> _nearObjs = new();
    [SerializeField] Transform _heldPieceContainer;
    [SerializeField] PieceRenderer _ghostPr;

    [Header("Spells")] 
    
    [SerializeField] SpellProjectileData _preparedSpellProjectile;
    [SerializeField] bool _canCast;
    [SerializeField] int _spellDmg;
    [SerializeField] int _spellAmmo;

    [Header("Shield")]
    
    [SerializeField] GameObject _shieldPivot;
    Transform _shieldTransform;

    void Awake() {
        _mainCamera = Camera.main;
        
        _playerMovement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();

        _shieldTransform = _shieldPivot.transform.GetChild(0);
        _fullShieldScale = _shieldTransform.localScale.x;
    }

    void Start() {
        _playerHealth.OnUpdateShield += ScaleShield;
    }

    void Update() {
        HandleGhostPiece();
        HandleShield();
    }

    public void Interact() {
        if (_heldPiece == null) {
            PickUp();
        } else {
            Drop();
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

    Coroutine _preparedSpellLifespan;
    public Action<float> OnUpdateSpellDuration;
    public bool PrepareSpell() {
        Vector2Int hoverPoint = GetHoverPoint();
        Block hoverBlock = _playerBoard.SelectPosition(hoverPoint.x, hoverPoint.y);
        if (hoverBlock == null || !hoverBlock.IsActive) return false; // hovering over nothing

        List<Block> spellBlocks = _playerBoard.FindColorBlockGroup(hoverPoint.x, hoverPoint.y);
        Color color = spellBlocks[0].Color;
        int power = spellBlocks.Count;
        print($"Spell Power: {power}");

        // TODO: delayed prepare with animation for "charging up"

        // Consume blocks part of spell
        _playerBoard.photonView.RPC(nameof(Board.S_DisableBlocks), RpcTarget.All, (object) spellBlocks.ToArray());

        // Execute spell
        // note: replace this with more robust system if expanding spell types
        SpellType spellType = GameManager.Instance.SpellColorDict[color];
        switch (spellType) {
            case SpellType.Damage:
                _preparedSpellProjectile = new() {Dmg = power, Speed = power / 2 + 1};

                // Start prepared spell lifespan
                if (_preparedSpellLifespan != null) StopCoroutine(_preparedSpellLifespan);
                _preparedSpellLifespan = StartCoroutine(PreparedSpellTimer());

                break;
            case SpellType.Shield:
                _playerHealth.ModifyShield(power);
                break;
            case SpellType.Speed:
                _playerMovement.MultiplySpeedForDuration(1f + (float)power / 30, Constants.SpellDuration);
                break;
            case SpellType.Wall:
                break;
            default:
                Debug.LogError($"Unable to execute spell: Unexpected SpellType {spellType.ToString()}");
                return false;
        }

        return true;
    }

    // Timer tracking how long Player can use their spell
    IEnumerator PreparedSpellTimer() {
        _canCast = true;

        float timer = Constants.SpellDuration;
        while (timer > 0) {
            timer -= Time.deltaTime;
            OnUpdateSpellDuration.Invoke(timer / Constants.SpellDuration);

            yield return null;
        }

        _canCast = false;
    }

    public void Cast() {
        if (!_canCast) return;

        // Set spell direction towards mouse
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 moveDir = (mouseWorldPosition - transform.position).normalized;
        _preparedSpellProjectile.MoveDir = moveDir;

        Vector2 startPos = moveDir * Constants.SpellInstantiatePosOffset + transform.position; // constant for spell spawn position offset

        Factory.Instance.photonView.RPC(nameof(Factory.S_CreateSpellObj), RpcTarget.AllViaServer, _preparedSpellProjectile, startPos);
    }

    #region Shield

    bool _isShielding;
    void HandleShield() {
        if (!_isShielding || _playerHealth.Shield <= 0) return;

        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 dir = mouseWorldPosition - transform.position; // direction from the player to the mouse

        // Calculate the angle in degrees between the player and the mouse, rotate shield to face mouse
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _shieldPivot.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

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
    public void S_ActivateShield() {
        _shieldPivot.SetActive(true);
    }
    [PunRPC]
    public void S_DeactivateShield() {
        _shieldPivot.SetActive(false);
    }

    #endregion

    #region Piece

    void PickUp() {
        if (_nearObjs.Count == 0) return;

        // Find nearest piece
        float minDistance = int.MaxValue;
        foreach (GameObject obj in _nearObjs.ToList()) {
            float d = Vector2.Distance(transform.position, obj.transform.position);
            if (d < minDistance && obj.TryGetComponent(out Piece piece)) {
                minDistance = d;
                _heldPiece = piece;
            }
        }

        // Pickup piece
        if (_heldPiece) {
            // Disable held piece's self movement
            if (_heldPiece.TryGetComponent(out MoveToPoint mtp)) {
                _heldPiece.photonView.RPC(nameof(MoveToPoint.Disable), RpcTarget.All);
            }

            // Setup ghost piece
            _ghostPr.Init(_heldPiece);

            // Take ownership of held piece
            // _heldPiece.photonView.RequestOwnership(); // server authoritative - requires Piece Ownership -> Request
            _heldPiece.photonView.TransferOwnership(photonView.Owner); // client authoritative - requires Piece Ownership -> Takeover

            // Make piece a child object of player
            NetworkManager.Instance.photonView.RPC(nameof(NetworkUtils.S_SetTransform), RpcTarget.All, _heldPiece.photonView.ViewID,
                _heldPieceContainer.localPosition, Quaternion.identity, photonView.ViewID, false);
        }
    }

    void Drop() {
        if (_heldPiece == null) return;

        Vector2Int hoverPoint = GetHoverPoint();

        if (_playerBoard.PlacePiece(_heldPiece, hoverPoint.x, hoverPoint.y)) {
            _heldPiece = null;
            return;
        } else {
            Debug.Log($"Unable to place block at ({hoverPoint.x}, {hoverPoint.y})");
        }

        // Check if trashing
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 100.0f, _interactableLayer);
        if (hit) {
            if (hit.collider.TryGetComponent(out Trash trash)) {
                trash.TrashObj(_heldPiece.gameObject);
                _heldPiece = null;
            }
        }
    }

    Vector2Int _lastHoverPoint = new Vector2Int(-1, -1);
    void HandleGhostPiece() {
        if (_heldPiece == null) {
            _ghostPr.gameObject.SetActive(false);
            return;
        }

        Vector2Int hoverPoint = GetHoverPoint();
        if (_lastHoverPoint == hoverPoint) return;
        _lastHoverPoint = hoverPoint;

        // Disable ghost piece if held piece is not over player's board
        if (_playerBoard.ValidatePiece(_heldPiece, hoverPoint.x, hoverPoint.y, _playerBoard.IsInBounds)) {
            _ghostPr.gameObject.SetActive(true);
        } else {
            _ghostPr.gameObject.SetActive(false);
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

    #endregion
}