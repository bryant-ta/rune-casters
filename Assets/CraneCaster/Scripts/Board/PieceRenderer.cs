using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Piece))]
public class PieceRenderer : MonoBehaviour {
    Piece _piece;

    Dictionary<Block, SpriteRenderer> _blockSprites = new();

    // Should usually call after initializing Piece
    public void Init(Piece piece) {
        _piece = piece;
        
        // Create SpriteRenderers for Piece's Blocks
        foreach (Block block in _piece.Blocks) {
            GameObject blockObj = Instantiate(Factory.BlockBase, _piece.transform, true);

            if (blockObj.TryGetComponent(out SpriteRenderer sr)) {
                _blockSprites[block] = blockObj.GetComponent<SpriteRenderer>();
            } else {
                Debug.LogError("Expected SpriteRender on Block base object");
            }
        }
        
        Render();
    }

    // Update render for Piece's Blocks
    public void Render() {
        foreach (Block block in _piece.Blocks) {
            SpriteRenderer sr = _blockSprites[block];
            sr.transform.localPosition = new Vector3(block.Position.x, block.Position.y, 0);
            sr.color = block.Color;
        }
    }
}