using Photon.Pun;
using UnityEngine;

public class PauseMenu : MonoBehaviourPunCallbacks {
	[SerializeField] GameObject _pauseMenuPanel;

	public void TogglePauseMenu() {
		_pauseMenuPanel.SetActive(!_pauseMenuPanel.activeSelf);
		GameManager.Instance.IsPaused = _pauseMenuPanel.activeSelf;
	}
	
	public void OnLeaveGameButtonClicked() {
		PhotonNetwork.Disconnect(); // TODO: change to just loading main menu again but still logged in
	}
}
