using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

// Online multiplayer PlayerInput. Prob replace with local multiplayer version here for local
public class PlayerInput : MonoBehaviourPun {
    Player _player;
    PlayerMovement _playerMovement;

    void Awake() {
        _player = GetComponent<Player>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        if (!photonView.IsMine) return;

        _playerMovement.moveInput = context.ReadValue<Vector2>();
    }

    // uses Action Type "Button"
    public void OnInteract(InputAction.CallbackContext ctx) {
        if (!photonView.IsMine) return;

        if (ctx.performed) {
            _player.Interact();
        }
    }
    
    public void OnRotate(InputAction.CallbackContext ctx) {
        if (!photonView.IsMine) return;

        if (ctx.performed) {
            _player.RotatePiece();
        }
    }
    
    public void OnAttack(InputAction.CallbackContext ctx) {
        if (!photonView.IsMine) return;

        if (ctx.performed) {
            _player.Attack();
        }
    }
    
    public void OnSelectTarget1(InputAction.CallbackContext ctx) {
        if (!photonView.IsMine) return;

        if (ctx.performed) {
            _player.SelectTarget(1);
        }
    }
    
    public void OnSelectTarget2(InputAction.CallbackContext ctx) {
        if (!photonView.IsMine) return;

        if (ctx.performed) {
            _player.SelectTarget(2);
        }
    }
    
    public void OnSelectTarget3(InputAction.CallbackContext ctx) {
        if (!photonView.IsMine) return;

        if (ctx.performed) {
            _player.SelectTarget(3);
        }
    }
}