using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Client : MonoBehaviourPunCallbacks {
    [SerializeField] string _gameVersion = "0.0.0";
    public string GameVersion => _gameVersion;

    [SerializeField] string _nickName = "Jaayced"; // TEMP: remove when player input for name

    public static Player MyPlayer;

    public string NickName {
        get {
            int value = Random.Range(0, 9999); // TEMP: remove when player input for name
            return _nickName + value;
        }
    }

    void Awake() {
        print("Connecting to server...");

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = NickName;
        PhotonNetwork.GameVersion = GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        print("Connected to server.");
        print(PhotonNetwork.LocalPlayer.NickName);

        if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        print("Joined lobby.");

        JoinRoom("TestRoom"); // TEMP: remove when player input for room
    }

    void JoinRoom(string roomName) {
        if (!PhotonNetwork.IsConnected) return;

        RoomOptions options = new RoomOptions();
        options.BroadcastPropsChangeToAll = true;
        options.MaxPlayers = 4;
        options.EmptyRoomTtl = 0;
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom() { print("Created room successfully."); }

    public override void OnCreateRoomFailed(short returnCode, string message) { print("Room creation failed" + message); }

    public override void OnDisconnected(DisconnectCause cause) { print("Disconnected from server for reason " + cause); }
}