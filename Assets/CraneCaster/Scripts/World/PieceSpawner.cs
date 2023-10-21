using Photon.Pun;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {
	[SerializeField] [Tooltip("Spawn every X seconds")] int _spawnRate = 10;
	[SerializeField] Transform _endPoint;

	float _timer = 99; // DEBUG: start spawning immediately
	public void Update() {
		if (!PhotonNetwork.IsMasterClient) return;
		
		if (_timer > _spawnRate) {
			SpawnPiece();
			
			_timer = 0f;
		}

		_timer += Time.deltaTime;
	}

	void SpawnPiece() {
		Piece piece = Factory.Instance.CreatePieceObj(GeneratePieceData(), transform.position);
		
		if (piece.TryGetComponent(out MoveToPoint mtp)) {
			mtp.SetMoveToPoint(_endPoint.position);
			mtp.OnReachedEnd += CleanUpPiece;
		}

		// TEMP: TODO: replace with drawn mini version of pieces
		piece.transform.localScale *= Constants.PieceShrinkFactor; // synced by PhotonTransformViewClassic
	}

	PieceData GeneratePieceData() {
		PieceData pieceData = PieceTypeLookUp.LookUp[Utils.GetRandomEnum<PieceType>()];
		pieceData.Color = GameManager.Instance.ColorRollTable.GetRandom();
		pieceData.CanRotate = true;

		return pieceData;
	}

	void CleanUpPiece(GameObject pieceObj) {
		pieceObj.GetComponent<MoveToPoint>().OnReachedEnd -= CleanUpPiece;
		PhotonNetwork.Destroy(pieceObj);
	}
}
