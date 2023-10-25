using System;
using TMPro;
using UnityEngine;

public class StartGameCountdown : MonoBehaviour {
	[SerializeField] TextMeshProUGUI _startGameCountdownText;

	void Start() {
		GameManager.Instance.StartGameCountdown.TickEvent += UpdateCountdownText;
	}

	public void UpdateCountdownText(float percent) {
		int second = (int) (percent * GameManager.Instance.StartGameDelay);
		_startGameCountdownText.text = second.ToString();

		if (second == 0) {
			GameManager.Instance.StartGameCountdown.TickEvent -= UpdateCountdownText;
			gameObject.SetActive(false);
		}
	}
}
