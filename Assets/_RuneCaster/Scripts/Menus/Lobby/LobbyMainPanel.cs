using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Based on DemoAsteroids Project from Photon Demos

namespace Lobby {
public class LobbyMainPanel : MonoBehaviourPunCallbacks {
    [Header("Login Panel")]
    public GameObject LoginPanel;

    public InputField PlayerNameInput;

    [Header("Selection Panel")]
    public GameObject SelectionPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public InputField RoomNameInputField;
    public InputField MaxPlayersInputField;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Room List Panel")]
    public GameObject RoomListPanel;

    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;

    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;

    Dictionary<string, RoomInfo> _cachedRoomList;
    Dictionary<string, GameObject> _roomListEntries;
    Dictionary<int, GameObject> _playerListEntries;

    #region UNITY

    public void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;

        _cachedRoomList = new Dictionary<string, RoomInfo>();
        _roomListEntries = new Dictionary<string, GameObject>();

        PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnConnectedToMaster() { this.SetActivePanel(SelectionPanel.name); }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby() {
        // whenever this joins a new lobby, clear any previous room lists
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
    public override void OnLeftLobby() {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnJoinedRoom() {
        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        _cachedRoomList.Clear();

        SetActivePanel(InsideRoomPanel.name);

        if (_playerListEntries == null) {
            _playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList) {
            GameObject entry = Instantiate(PlayerListEntryPrefab, InsideRoomPanel.transform, true);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Init(p.ActorNumber, p.NickName);

            object isPlayerReady; // TODO: determine why this needs to be an object?
            if (p.CustomProperties.TryGetValue(CustomPropertiesKey.PlayerReady, out isPlayerReady)) {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
            }

            _playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable {
            {CustomPropertiesKey.PlayerLoaded, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom() {
        SetActivePanel(SelectionPanel.name);

        foreach (GameObject entry in _playerListEntries.Values) {
            Destroy(entry.gameObject);
        }

        _playerListEntries.Clear();
        _playerListEntries = null;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { SetActivePanel(SelectionPanel.name); }

    public override void OnJoinRoomFailed(short returnCode, string message) { SetActivePanel(SelectionPanel.name); }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions {MaxPlayers = 8};

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(InsideRoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Init(newPlayer.ActorNumber, newPlayer.NickName);

        _playerListEntries.Add(newPlayer.ActorNumber, entry);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Destroy(_playerListEntries[otherPlayer.ActorNumber].gameObject);
        _playerListEntries.Remove(otherPlayer.ActorNumber);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber) {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (_playerListEntries == null) {
            _playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry)) {
            object isPlayerReady;
            if (changedProps.TryGetValue(CustomPropertiesKey.PlayerReady, out isPlayerReady)) {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    #region UI CALLBACKS

    public void OnBackButtonClicked() {
        if (PhotonNetwork.InLobby) {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(SelectionPanel.name);
    }

    public void OnCreateRoomButtonClicked() {
        string roomName = RoomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        byte maxPlayers;
        byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 10000};

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnJoinRandomRoomButtonClicked() {
        SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked() { PhotonNetwork.LeaveRoom(); }

    public void OnLoginButtonClicked() {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals("")) {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        } else {
            Debug.LogError("Player Name is invalid.");
        }
    }

    public void OnRoomListButtonClicked() {
        if (!PhotonNetwork.InLobby) {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }

    public void OnStartGameButtonClicked() {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("MainScene");
    }

    #endregion

    bool CheckPlayersReady() {
        if (!PhotonNetwork.IsMasterClient) {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList) {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(CustomPropertiesKey.PlayerReady, out isPlayerReady)) {
                if (!(bool) isPlayerReady) {
                    return false;
                }
            } else {
                return false;
            }
        }

        return true;
    }

    void ClearRoomListView() {
        foreach (GameObject entry in _roomListEntries.Values) {
            Destroy(entry.gameObject);
        }

        _roomListEntries.Clear();
    }

    public void LocalPlayerPropertiesUpdated() { StartGameButton.gameObject.SetActive(CheckPlayersReady()); }

    void SetActivePanel(string activePanel) {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name)); // UI should call OnRoomListButtonClicked() to activate this
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }

    void UpdateCachedRoomList(List<RoomInfo> roomList) {
        foreach (RoomInfo info in roomList) {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {
                if (_cachedRoomList.ContainsKey(info.Name)) {
                    _cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (_cachedRoomList.ContainsKey(info.Name)) {
                _cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else {
                _cachedRoomList.Add(info.Name, info);
            }
        }
    }

    void UpdateRoomListView() {
        foreach (RoomInfo info in _cachedRoomList.Values) {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte) info.PlayerCount, (byte) info.MaxPlayers);

            _roomListEntries.Add(info.Name, entry);
        }
    }
}
}