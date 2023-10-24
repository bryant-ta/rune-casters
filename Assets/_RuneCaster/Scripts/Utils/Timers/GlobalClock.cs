using System;
using UnityEngine;

/// <summary>
/// GlobalClock is the ticker every other Timer type uses to tick. All Timers can be stopped by stopping GlobalClock.
/// </summary>
public class GlobalClock : MonoBehaviour {
	public static GlobalClock Instance { get; private set; }

	public static float TimeScale = 1f;

	/// <summary>
	/// OnTick invokes with deltaTime.
	/// </summary>
	public static Action<float> onTick;

	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
		}
			
		onTick = null; // null is same as saying onTick has no subscribers
	}

	void Update() {
		if (TimeScale == 0) return; // prevents ticks of 0 delta time
		onTick?.Invoke(Time.deltaTime * TimeScale);
	}
}
