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
		int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
		Player player = NetworkManager.Instance.PlayerList[playerId];
		player.PlayerId = playerId;
		Client.MyPlayer = player;

		// Enable Player on all clients
		NetworkManager.Instance.photonView.RPC(nameof(NetworkManager.EnablePlayerObj), RpcTarget.AllBuffered, playerId); // care for calling this often, buffered
		
		// Set Player object ownership
		if (!player.photonView.IsMine) {
			player.photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // client authoritative - requires Player Ownership -> Takeover
		}
	}

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
		// Set PlayerID for each Player instance on each client
		int playerId = newPlayer.ActorNumber;
		Player player = NetworkManager.Instance.PlayerList[playerId];
		player.PlayerId = playerId;
		// possible problem with 3 players for making sure PlayerID is synced across all 3 clients
	}
}
