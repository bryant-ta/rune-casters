using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer : MonoBehaviour {
	Board _board;
	
	Dictionary<Block, SpriteRenderer> _blockSprites = new();

	public void Init(Board board) {
		_board = board;
		
		// Create SpriteRenderers for Board's Blocks
		foreach (Block block in _board.Blocks) {
			GameObject blockObj = Instantiate(PieceFactory.BlockBase, _board.transform, true);

			if (blockObj.TryGetComponent(out SpriteRenderer sr)) {
				_blockSprites[block] = blockObj.GetComponent<SpriteRenderer>();
			} else {
				Debug.LogError("Expected SpriteRender on Block base object");
			}
		}
        
		Render();
	}

	// Update render for Board's Blocks
	// Note: inefficency updating whole board at once every time
	public void Render() {
		foreach (Block block in _board.Blocks) {
			SpriteRenderer sr = _blockSprites[block];
			sr.transform.localPosition = new Vector3(block.Position.x, block.Position.y, 0);
			sr.color = block.Color;

			sr.gameObject.SetActive(block.IsActive);
		}
	}
}
