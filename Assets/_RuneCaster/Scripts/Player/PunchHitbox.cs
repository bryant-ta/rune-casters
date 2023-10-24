using Game;
using Timers;
using UnityEngine;

// Attach to Punch collider
public class PunchHitbox : MonoBehaviour {
	[SerializeField] float _duration;

	public Player Player => _player;
	[SerializeField] Player _player;
	
	CountdownTimer _activeDuration;

	void Awake() {
		_activeDuration = new CountdownTimer(_duration);
		_activeDuration.EndEvent += EndActiveDuration;
	}

	void EndActiveDuration() {
		gameObject.SetActive(false);
	}

	public void OnEnable() {
		_activeDuration.Start();
	}
	public void OnDisable() {
		_activeDuration.Stop();
	}
}
