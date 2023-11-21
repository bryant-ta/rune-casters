using System;
using System.Collections.Generic;
using UnityEngine;

public class HeldPieceOutliner : MonoBehaviour {
	[SerializeField] Color _outlineColor;
	
	[SerializeField] List<SpriteRenderer> blockOutlinePool = new();
	[SerializeField] GameObject _blockOutlinePrefab;

	void Awake() {
		foreach (SpriteRenderer blockSr in blockOutlinePool) {
			blockSr.color = _outlineColor;
		}
	}

	// Update block outline effect to fit held piece
	public void RefreshOutline(Piece heldPiece) {
		// Reset outline
		foreach (SpriteRenderer blockSr in blockOutlinePool) {
			blockSr.gameObject.SetActive(false);
			blockSr.transform.SetParent(transform);
		}

		// Match block outlines to held piece shape
		if (heldPiece != null) {
			List<GameObject> heldPieceBlockObj = heldPiece.GetPieceBlockObjs();
			for (int i = 0; i < heldPiece.Shape.Count; i++) {
				SpriteRenderer blockSr = GetBlockOutline(i);

				blockSr.transform.SetParent(heldPieceBlockObj[i].transform);
				blockSr.transform.localPosition = Vector3.zero;
				blockSr.gameObject.SetActive(true);
			}
		}
	}

	SpriteRenderer GetBlockOutline(int index) {
		if (index + 1 > blockOutlinePool.Count) {
			SpriteRenderer blockSr = Instantiate(_blockOutlinePrefab, transform).GetComponent<SpriteRenderer>();
			blockSr.color = _outlineColor;
			
			blockOutlinePool.Add(blockSr);
			return blockSr;
		}

		return blockOutlinePool[index];
	}
}
