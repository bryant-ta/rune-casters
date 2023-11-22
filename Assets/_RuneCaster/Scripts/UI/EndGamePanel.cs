using System;
using Photon.Pun;
using UnityEngine;

public class EndGamePanel : MonoBehaviour {
	[SerializeField] GameObject _endGamePanel;

	void Start() {
		GameManager.Instance.OnEndGame += ToggleEndGamePanel;
	}

	public void ToggleEndGamePanel() {
		_endGamePanel.SetActive(!_endGamePanel.activeSelf);
	}
	
	public void OnLeaveGameButtonClicked() {
		PhotonNetwork.Disconnect(); // TODO: change to just loading main menu again but still logged in
	}
	public void OnRestartGameButtonClicked() {
		
	}
}
