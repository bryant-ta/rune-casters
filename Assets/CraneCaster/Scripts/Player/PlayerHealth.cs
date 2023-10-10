using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerUI))]
public class PlayerHealth : MonoBehaviour {
	public int Hp => _hp;
	[SerializeField] int _hp;
	public int MaxHp => _maxHp;
	[SerializeField] int _maxHp;

	PlayerUI _playerUI;

	void Awake() {
		_playerUI = GetComponent<PlayerUI>();
	}

	/// <summary>
	/// Changes player health.
	/// </summary>
	/// <param name="value">Positive input heals. Negative input damages.</param>
	/// <returns>Actual health value delta including sign</returns>
	public int ModifyHp(int value) {
		int newHp = _hp + value;
		if (newHp <= 0) {
			newHp = 0;
		} else if (newHp > _maxHp) {
			newHp = _maxHp;
		}

		int delta = newHp - _hp;
		_hp = newHp;

		_playerUI.UpdateHpBar((float) _hp / _maxHp);
		
		Hashtable properties = new Hashtable() {{"Hp", _hp}};
		PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
		
		// Death
		if (_hp == 0) {
			Debug.Log("Player died");
		}

		return delta;
	}
}
