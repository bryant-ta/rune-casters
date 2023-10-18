using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance { get; private set; }

	public List<SpellColor> SpellColors = new(); // Builds into SpellColorDict at runtime
	public Dictionary<Color, SpellType> SpellColorDict = new();
	
	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
		} else {
			Instance = this;
		}
		
		// Build SpellColorDict
		foreach (SpellColor spellColor in SpellColors) {
			SpellColorDict[spellColor.Color] = spellColor.Type;
		}
	}
}