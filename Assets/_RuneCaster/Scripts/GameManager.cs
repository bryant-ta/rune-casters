using System;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance { get; private set; }

	[SerializeField] List<SpellColor> SpellColors = new(); // for setting colors in inspector
	public Dictionary<Color, SpellType> SpellColorDict { get; private set; }
	public RollTable<Color> ColorRollTable { get; private set; }

	public int StartGameDelay;
	public CountdownTimer StartGameCountdown { get; private set; }

	public bool IsPaused;

	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
		}
		
		// Build SpellColorDict and piece color roll table
		SpellColorDict = new Dictionary<Color, SpellType>();
		ColorRollTable = new RollTable<Color>();
		foreach (SpellColor spellColor in SpellColors) {
			ColorRollTable.Add(spellColor.Color, spellColor.SpawnPercentChance);
			SpellColorDict[spellColor.Color] = spellColor.Type;
		}
		
		StartGameCountdown = new CountdownTimer(StartGameDelay);
		IsPaused = true;
	}

	public void StartInitialCountdown() {
		print("countdown started");
		StartGameCountdown.Start();
		StartGameCountdown.EndEvent += StartGame;
	}

	public void StartGame() {
		Debug.Log("Starting game!");
		StartGameCountdown.EndEvent -= StartGame;
		IsPaused = false;
	}
}