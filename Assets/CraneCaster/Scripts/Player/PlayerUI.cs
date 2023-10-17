using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player), typeof(PlayerHealth))]
public class PlayerUI : MonoBehaviourPunCallbacks {
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
	
	public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
		print("a");
		if (_player.PlayerId != targetPlayer.ActorNumber) return;
		print("b");
		
		if (changedProps[CustomPropertiesLookUp.LookUp[CustomPropertiesKey.SpellLifespan]] != null) { 
			UpdateSpellLifespanBar((float) changedProps[CustomPropertiesLookUp.LookUp[CustomPropertiesKey.SpellLifespan]]);
			print("c");
		}
	}
}
