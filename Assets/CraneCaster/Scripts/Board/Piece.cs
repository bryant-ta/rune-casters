using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Piece : MonoBehaviourPun, IPunInstantiateMagicCallback {
    public List<Vector2Int> Shape => _shape;
    [SerializeField] List<Vector2Int> _shape;

    public Color Color => _color;
    [SerializeField] Color _color;

    public bool CanRotate => _canRotate;
    [SerializeField] bool _canRotate;

    public List<Block> Blocks => _blocks;
    List<Block> _blocks = new();

    PieceRenderer _pr;

    public void Init(PieceData pieceData) {
        _shape = pieceData.Shape;
        _color = pieceData.Color;
        _canRotate = pieceData.CanRotate;

        // Populate Block list
        foreach (Vector2Int blockOffset in pieceData.Shape) {
            Block block = new Block(blockOffset, pieceData.Color, true);
            _blocks.Add(block);
        }

        // Init PieceRenderer with populated Block list
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

        _pr.Render();
    }

    // Rotate counterclockwise
    public void RotateCCW() {
        if (!_canRotate) return;

        foreach (Block block in _blocks) {
            // Simplified rotation assuming pivot point is always (0,0)
            int newX = -block.Position.y;
            int newY = block.Position.x;

            block.Position = new Vector2Int(newX, newY);
        }

        _pr.Render();
    }

    // Rotates the current piece clockwise.
    // public bool RotatePiece()
    // {
    // 	if (!piece.canRotate)
    // 	{
    // 		return false;
    // 	}
    //
    // 	Dictionary<Block, Position> piecePosition = piece.GetPositions();
    // 	var offset = piece.blocks[0].Position;
    //
    // 	foreach (var block in piece.blocks)
    // 	{
    // 		var row = block.Position.Row - offset.Row;
    // 		var column = block.Position.Column - offset.Column;
    // 		block.MoveTo(-column + offset.Row, row + offset.Column);
    // 	}
    //
    // 	if (HasCollisions() && !ResolveCollisionsAfterRotation())
    // 	{
    // 		RestoreSavedPiecePosition(piecePosition);
    // 		return false;
    // 	}
    // 	return true;
    // }
}