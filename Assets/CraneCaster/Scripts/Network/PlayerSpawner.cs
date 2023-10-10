using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks {
	[SerializeField] GameObject _playerObj;
	[SerializeField] Transform[] _spawnPositions;

	public override void OnJoinedRoom() {
		// Check max players
		int curNumPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
		if (curNumPlayers > 4) { // TODO: pull from Photon room info
			Debug.LogError("Failed to spawn player: room is full");
			return;
		}

		// Setup Player
		int playerId = curNumPlayers - 1;
		Player player = GameManager.Instance.PlayerList[playerId];
		player.PlayerId = playerId;

		// Enable Player on all clients
		GameManager.Instance.photonView.RPC(nameof(GameManager.EnablePlayerObj), RpcTarget.AllBuffered, playerId); // care for calling this often, buffered
		
		// Set Player object ownership
		if (!player.photonView.IsMine) {
			player.photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // client authoritative - requires Player Ownership -> Takeover
		}
	}
}
