using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviourPun {
    public int PlayerId;
    
    [SerializeField] Piece _heldPiece;
    [SerializeField] LayerMask _interactableLayer;

    [SerializeField] Board _playerBoard;
    PlayerHealth _attackTarget;

    List<GameObject> _nearObjs = new();
    
    public void Interact() {
        if (_heldPiece == null) {
            PickUp();
        } else {
            Drop();
        }
    }

    public void RotatePiece() {
        if (_heldPiece) _heldPiece.photonView.RPC(nameof(Piece.RotateCW), RpcTarget.All);
    }
    
    public void Attack() {
        if (_attackTarget) {
            int dmg = _playerBoard.CalculateBoardDamage();
            _attackTarget.ModifyHp(-dmg);
            print($"Did {dmg} damage to Player {_attackTarget}");
        }
    }

    public void SelectTarget(int targetPlayer) {
        if (targetPlayer > PhotonNetwork.CurrentRoom.PlayerCount) {
            Debug.Log($"Selected target {targetPlayer}, but there are only {PhotonNetwork.CurrentRoom.PlayerCount} players in the room.");
            return;
        }
        
        // should prob to cache/only update this when player list changes
        List<PlayerHealth> otherPlayers = new();
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerListOthers) {
            print("trying " + p.ActorNumber);
            // GameObject playerObj = (GameObject) p.TagObject;
            // otherPlayers.Add(playerObj.GetComponent<PlayerHealth>());
        }
        
        _attackTarget = otherPlayers[targetPlayer];
        print($"Selected Player {_attackTarget} as target");
    }

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
            if (_heldPiece.TryGetComponent(out MoveToPoint mtp)) {
                _heldPiece.photonView.RPC(nameof(MoveToPoint.Disable), RpcTarget.All);
            }

            // Take ownership of held piece
            // _heldPiece.photonView.RequestOwnership(); // server authoritative - requires Piece Ownership -> Request
            _heldPiece.photonView.TransferOwnership(photonView.Owner); // client authoritative - requires Piece Ownership -> Takeover

            // Make piece a child object of player
            GameManager.Instance.photonView.RPC(nameof(NetworkUtils.S_SetTransform), RpcTarget.All, _heldPiece.photonView.ViewID,
                Vector3.zero, Quaternion.identity, photonView.ViewID, false);
        }
    }

    void Drop() {
        if (_heldPiece == null) return;

        // localPosition works when Player is child of Board and centered at origin
        Vector2Int hoverPoint = new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));

        // print("Hoverpoint" + hoverPoint);

        if (_playerBoard.PlacePiece(_heldPiece, hoverPoint)) {
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

    void OnTriggerEnter2D(Collider2D col) {
        if (!_nearObjs.Contains(col.gameObject)) {
            _nearObjs.Add(col.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (_nearObjs.Contains(col.gameObject)) {
            _nearObjs.Remove(col.gameObject);
        }
    }
    
    public void Enable() {
        _playerBoard.gameObject.SetActive(true);
        enabled = true;
    }
    public void Disable() {
        _playerBoard.gameObject.SetActive(false);
        enabled = false;
    }
}