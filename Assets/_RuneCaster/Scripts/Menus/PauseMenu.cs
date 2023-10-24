using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks {
	[SerializeField] GameObject _pauseMenuPanel;

	public bool IsPaused { get; private set; }

	public void OnTogglePauseMenu() {
		_pauseMenuPanel.SetActive(!_pauseMenuPanel.activeSelf);
		IsPaused = _pauseMenuPanel.activeSelf;
	}
	
	public void OnLeaveGameButtonClicked() {
		PhotonNetwork.Disconnect(); // TODO: change to just loading main menu again but still logged in
	}

	public override void OnDisconnected(DisconnectCause cause) {
		SceneManager.LoadScene("LobbyScene");
	}
}
