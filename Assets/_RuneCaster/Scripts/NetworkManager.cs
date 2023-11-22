using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Player = Game.Player;

public class NetworkManager : MonoBehaviourPunCallbacks {
    public static NetworkManager Instance { get; private set; }

    public List<Player> PlayerList => _playerList.ToList();
    [SerializeField] List<Player> _playerList = new(); // players registered when client joins, see PlayerSpawner

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        // IMPORTANT: this aligns game's PlayerID with Photon's ActorID, so we don't have to do ActorID - 1 everywhere. kinda jank
        _playerList.Insert(0, null);
    }

    void Start() { StartCoroutine(AfterConnected()); }

    IEnumerator AfterConnected() {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);

        // Setup Player
        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        Player player = PlayerList[playerId];
        player.PlayerId = playerId;
        Debug.Log($"Player {playerId} joined the game.");

        // Enable Player on all clients
        photonView.RPC(nameof(EnablePlayerObj), RpcTarget.AllBuffered, playerId); // care for calling this often, buffered

        // Set Player object ownership
        if (!player.photonView.IsMine) {
            player.photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // client authoritative - requires Player Ownership -> Takeover
        }

        // Send ready signal
        Hashtable props = new Hashtable {{CustomPropertiesKey.PlayerLoaded, true}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnDisconnected(DisconnectCause cause) { SceneManager.LoadScene("LobbyScene"); }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
        // Start countdown for beginning game after all players are ready
        if (changedProps.ContainsKey(CustomPropertiesKey.PlayerLoaded)) {
            bool allPlayersLoaded = true;
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList) {
                if (p.CustomProperties.TryGetValue(CustomPropertiesKey.PlayerLoaded, out object playerLoadedLevel)) {
                    if ((bool) playerLoadedLevel) {
                        continue;
                    }
                }

                allPlayersLoaded = false;
                break;
            }

            if (allPlayersLoaded) {
                GameManager.Instance.StartInitialCountdown();
            } else {
                Debug.Log("Waiting for all players to load.");
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        // Set PlayerID for each Player instance on each client
        int playerId = newPlayer.ActorNumber;
        Player player = PlayerList[playerId];
        player.PlayerId = playerId;
        // possible problem with 3 players for making sure PlayerID is synced across all 3 clients
    }

    [PunRPC]
    public void EnablePlayerObj(int playerId) { _playerList[playerId].Enable(); }
    [PunRPC]
    public void DisablePlayerObj(int playerId) { _playerList[playerId].Disable(); }

    // TODO: implement call to unregister when player disconnects
    public void UnregisterPlayer(Player player) {
        // possibly dont need to register if "!_playerList.Contains(player)" works in RegisterPlayer
        // must keep each player index constant
    }
}