using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviourPun {
    public int PlayerId;

    [SerializeField] int _spellDmg;
    [SerializeField] int _spellAmmo;

    [SerializeField] LayerMask _interactableLayer;
    [SerializeField] Board _playerBoard;

    Piece _heldPiece;
    List<GameObject> _nearObjs = new();

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
            return true;
        } 
        
        return false;
    }

    public void Cast() {
        if (_spellAmmo > 0) {
            _spellAmmo--;
            // ModifyHp(-dmg);
        }
    }

    public void PrepareSpell() {
        Vector2Int hoverPoint = GetHoverPoint();
        BuildSpell(hoverPoint.x, hoverPoint.y);
    }

    int BuildSpell(int hoverX, int hoverY) {
        // TODO: implement board power scoring based on matching color groups and size
        int power = _playerBoard.CountBlockGroup(hoverX, hoverY);
        print($"Spell Power: {power}");
        return power;
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

        Vector2Int hoverPoint = GetHoverPoint();
        // print("Hoverpoint" + hoverPoint);

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

    Vector2Int GetHoverPoint() {
        // localPosition works when Player is child of Board and centered at origin
        return new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
    }
}