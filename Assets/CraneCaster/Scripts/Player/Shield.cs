using System;
using Photon.Pun;
using UnityEngine;

public class Shield : MonoBehaviour {
	[SerializeField] PlayerHealth _playerHealth;

	// trigger detection must be here in script attached to actual shield collider
	public void OnTriggerEnter2D(Collider2D col) {
		if (!PhotonNetwork.IsMasterClient) return;

		if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Spell])) {
			SpellProjectile spellProjectile = col.gameObject.GetComponent<SpellProjectile>();
			_playerHealth.ModifyShield(-spellProjectile.Dmg);
		}
	}
}
