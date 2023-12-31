using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceRenderer : MonoBehaviour {
    Piece _piece;
    [SerializeField] Transform _blockContainer;

    Dictionary<Block, SpriteRenderer> _blockSprites = new();

    // Should usually call after initializing Piece
    // This Init can be called multiple times to point PieceRenderer at a new Piece
    public void Init(Piece piece) {
        // Reset block sprites
        foreach (KeyValuePair<Block,SpriteRenderer> blockRenderers in _blockSprites) {
            Destroy(blockRenderers.Value.gameObject);
        }
        _blockSprites.Clear();
        
        // Create SpriteRenderers for Piece's Blocks
        if (_piece != null) _piece.OnRender -= Render;
        _piece = piece;
        foreach (Block block in _piece.Blocks) {
            GameObject blockObj = Instantiate(Factory.BlockBase, _blockContainer, true);

            if (blockObj.TryGetComponent(out SpriteRenderer sr)) {
                _blockSprites[block] = sr;
                _blockSprites[block].sprite = GameManager.Instance.GetSpellTypeSprite(block.SpellType);
            } else {
                Debug.LogError("Expected SpriteRender on Block base object");
            }
        }
        
        Render();

        _piece.OnRender += Render;
    }

    // Update render for Piece's Blocks
    public void Render() {
        foreach (Block block in _piece.Blocks) {
            SpriteRenderer sr = _blockSprites[block];
            sr.transform.localPosition = new Vector3(block.Position.x, block.Position.y, 0);
        }
    }

    public void SetBlockOverlay() {
        foreach (var blockSprite in _blockSprites) {
            Color c = blockSprite.Value.color;
            c.a = Constants.PiecePlacementOverlayAlpha;
            blockSprite.Value.color = c;
        }
    }
    public void RemoveBlockOverlay() {
        foreach (var blockSprite in _blockSprites) {
            Color c = blockSprite.Value.color;
            c.a = 1;
            blockSprite.Value.color = c;
        }
    }

    public void SetEnableRender(bool doRender) { _blockContainer.gameObject.SetActive(doRender); }
    public List<GameObject> GetPieceBlockObjs() {
        List<GameObject> blockObjs = new();
        foreach (var blockSprite in _blockSprites) {
            blockObjs.Add(blockSprite.Value.gameObject);
        }

        return blockObjs;
    }
}