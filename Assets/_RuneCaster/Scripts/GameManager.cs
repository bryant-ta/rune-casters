using System;
using System.Collections.Generic;
using System.Linq;
using Timers;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {
	public static GameManager Instance { get; private set; }

	[SerializeField] List<PieceSpawnerRollEntry> PieceDataConfig = new(); // for setting colors in inspector
	public RollTable<SpellType> PieceSpawnerRollTable { get; private set; }

	public int StartGameDelay;
	public CountdownTimer StartGameCountdown { get; private set; }

	public bool IsPaused;

	[SerializeField] bool _debug;

	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
		}
		
		// Build ColorRollTable
		PieceSpawnerRollTable = new RollTable<SpellType>();
		foreach (PieceSpawnerRollEntry pieceChances in PieceDataConfig) {
			PieceSpawnerRollTable.Add(pieceChances.PieceSpellType, pieceChances.SpawnPercentChance);
		}
		
		StartGameCountdown = new CountdownTimer(StartGameDelay);
		IsPaused = true;
		
		if (_debug) IsPaused = false;
	}

	public void StartInitialCountdown() {
		if (_debug) return;
		
		print("countdown started");
		StartGameCountdown.Start();
		StartGameCountdown.EndEvent += StartGame;
	}

	public void StartGame() {
		Debug.Log("Starting game!");
		StartGameCountdown.EndEvent -= StartGame;
		IsPaused = false;
	}

	public void CheckEndGame() {
		
	}

	public void EndGame() {
		
	}

	public Sprite GetSpellTypeSprite(SpellType spellType) {
		if (spellType == SpellType.None) return null;
		
		return PieceDataConfig.Single(x => x.PieceSpellType == spellType).PieceSprite;
	}
}