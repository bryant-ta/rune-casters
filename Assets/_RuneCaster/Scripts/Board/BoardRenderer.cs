using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer : MonoBehaviour {
	Board _board;
	[SerializeField] Transform _blockContainer;
	[SerializeField] SpriteRenderer _boardSprite;
	
	Dictionary<Block, SpriteRenderer> _blockSprites = new();

	public void Init(Board board) {
		_board = board;
		
		// Create SpriteRenderers for Board's Blocks
		foreach (Block block in _board.Blocks) {
			GameObject blockObj = Instantiate(Factory.BlockBase, _blockContainer.transform, true);

			if (blockObj.TryGetComponent(out SpriteRenderer sr)) {
				_blockSprites[block] = blockObj.GetComponent<SpriteRenderer>();
				sr.sortingOrder = -9;
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
			_blockSprites[block].sprite = GameManager.Instance.GetSpellTypeSprite(block.SpellType);
			
			sr.gameObject.SetActive(block.IsActive);
		}
	}

	public void ChangeOpacity(float alpha) {
		Color c = _boardSprite.color;
		c.a = alpha;
		_boardSprite.color = c;
	}
}
