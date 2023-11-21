using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Piece : MonoBehaviourPun, IPunInstantiateMagicCallback {
    public List<Vector2Int> Shape => _shape;
    [SerializeField] List<Vector2Int> _shape;
    public SpellType SpellType => _spellType;
    [SerializeField] SpellType _spellType;
    public bool CanRotate => _canRotate;
    [SerializeField] bool _canRotate;

    public List<Block> Blocks => _blocks;
    List<Block> _blocks = new();

    PieceRenderer _pr;

    public Action OnRender;

    public void Init(PieceData pieceData) {
        _shape = pieceData.Shape;
        _spellType = pieceData.SpellType;
        _canRotate = pieceData.CanRotate;

        // Populate Block list
        foreach (Vector2Int blockOffset in pieceData.Shape) {
            Block block = new Block(blockOffset, pieceData.SpellType, true);
            _blocks.Add(block);
        }

        // Init main PieceRenderer with populated Block list (ghost PieceRenderer inited on pick up)
        if (TryGetComponent(out PieceRenderer pr)) {
            _pr = pr;
            _pr.Init(this);
        }
    }

    // Note: this is not called if instantiated network object is not root
    public void OnPhotonInstantiate(PhotonMessageInfo info) { Init((PieceData) info.photonView.InstantiationData[0]); }

    // Rotate clockwise
    [PunRPC]
    public void RotateCW() {
        if (!_canRotate) return;

        foreach (Block block in _blocks) {
            // Simplified clockwise rotation assuming pivot point is always (0,0)
            int newX = block.Position.y;
            int newY = -block.Position.x;

            block.Position = new Vector2Int(newX, newY);
        }

        OnRender.Invoke();
    }

    // Rotate counterclockwise
    public void RotateCCW() {
        if (!_canRotate) return;

        foreach (Block block in _blocks) {
            // Simplified counterclockwise rotation assuming pivot point is always (0,0)
            int newX = -block.Position.y;
            int newY = block.Position.x;

            block.Position = new Vector2Int(newX, newY);
        }

        OnRender.Invoke();
    }

    public void SetEnableRender(bool doRender) { _pr.SetEnableRender(doRender); }
    public List<GameObject> GetPieceBlockObjs() { return _pr.GetPieceBlockObjs(); }
}