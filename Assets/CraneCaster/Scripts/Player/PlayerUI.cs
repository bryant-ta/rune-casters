using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player), typeof(PlayerHealth))]
public class PlayerUI : MonoBehaviour {
	[SerializeField] Image _hpBar;
	[SerializeField] Image _spellLifespanBar;

	Player _player;
	PlayerHealth _playerHealth;

	void Awake() {
		_player = GetComponent<Player>();
		_playerHealth = GetComponent<PlayerHealth>();

		_player.OnUpdateSpellLifeSpan += UpdateSpellLifespanBar;
		_playerHealth.OnUpdateHp += UpdateHpBar;
	}

	void UpdateHpBar(float percent) {
		_hpBar.fillAmount = percent;
	}
	
	void UpdateSpellLifespanBar(float percent) {
		_spellLifespanBar.fillAmount = percent;
	}
}
