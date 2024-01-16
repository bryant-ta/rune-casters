using Photon.Pun;
using TMPro;
using UnityEngine;

// TODO: replace with colored icon (green, yellow, red) or a tab ping screen
public class PingDisplay : MonoBehaviour {
	TextMeshProUGUI _pingText;

	void Awake() {
		_pingText = GetComponent<TextMeshProUGUI>();
	}

	float _timer = int.MaxValue;
	float _pingUpdateInterval = 5f;
	void Update() {
		if (_timer > _pingUpdateInterval) {
			_timer = 0;
			_pingText.text = PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime.ToString();
		}

		_timer += Time.deltaTime;
	}
}
