using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun {
	public static GameManager Instance { get; private set; }

	public List<Player> PlayerList => _playerList.ToList();
	[SerializeField] List<Player> _playerList = new(); // players registered when client joins

	[SerializeField] bool _canStart;

	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
		}
	}

	// public void UpdatePlayerList() {
	// 	foreach (Player player in _playerList) {
	// 		
	// 	}
	// 	
	// 	
	// 	// can optimize later by keeping all Player objects in scene and enabling + assigning ownership when client joins
	// 	foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player")) {
	// 		Player player = playerObj.GetComponent<Player>();
	// 		if (!_playerList.ContainsKey(player.PlayerId)) {
	// 			_playerList[player.PlayerId] = player;
	// 		}
	// 	}
	// 	
	// 	//debug
	// 	print("PlayerList: ");
	// 	foreach (var player in _playerList) {
	// 		print(player.Value.PlayerId);
	// 	}
	// }

	[PunRPC]
	public void EnablePlayerObj(int playerId) {
		_playerList[playerId].Enable();
	}
	
	// TODO: implement call to unregister when player disconnects
	public void UnregisterPlayer(Player player) {
		// possibly dont need to register if "!_playerList.Contains(player)" works in RegisterPlayer
		// must keep each player index constant
	}
}
