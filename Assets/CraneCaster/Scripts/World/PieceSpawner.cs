using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PieceSpawner : MonoBehaviour {
	[SerializeField] [Tooltip("Spawn every X seconds")] int _spawnRate = 10;
	[SerializeField] Transform _endPoint;
	
	[SerializeField] List<Color> _pieceColors = new();

	float _timer;
	public void Update() {
		if (!PhotonNetwork.IsMasterClient) return;
		
		if (_timer > _spawnRate) {
			SpawnPiece();
			
			_timer = 0f;
		}

		_timer += Time.deltaTime;
	}

	void SpawnPiece() {
		Piece piece = PieceFactory.Instance.CreatePieceObj(GeneratePieceData(), transform.position);
		if (piece.TryGetComponent(out MoveToPoint mtp)) {
			mtp.SetMoveToPoint(_endPoint.position);
			mtp.OnReachedEnd += CleanUpPiece;
		}
	}

	PieceData GeneratePieceData() {
		PieceData pieceData = PieceTypeLookUp.LookUp[Utils.GetRandomEnum<PieceType>()];
		pieceData.Color = _pieceColors[Random.Range(0, _pieceColors.Count)];
		pieceData.CanRotate = true;

		return pieceData;
	}

	void CleanUpPiece(GameObject pieceObj) {
		pieceObj.GetComponent<MoveToPoint>().OnReachedEnd -= CleanUpPiece;
		PhotonNetwork.Destroy(pieceObj);
	}
}
