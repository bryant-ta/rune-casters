using System;
using Photon.Pun;
using UnityEngine;

// Attach to Shield collider
public class ShieldHitbox : MonoBehaviour {
	[SerializeField] PlayerHealth _playerHealth;
	
	// note: prob don't move shield logic here since this object is being disabled/enabled frequently

	// trigger detection must be here in script attached to actual shield collider to detect properly
	public void OnTriggerEnter2D(Collider2D col) {
		if (!PhotonNetwork.IsMasterClient) return;

		// note: if I were to do this again, prob let attack triggers be responsible to notify Player for dmg.
		// This is bc there can be many attack types but usually one/few Player colliders
		if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Spell])) {
			SpellProjectile spellProjectile = col.gameObject.GetComponent<SpellProjectile>();
			_playerHealth.ModifyShield(-spellProjectile.Dmg);
		}
		if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Punch])) {
			PunchHitbox punchHitbox = col.gameObject.GetComponent<PunchHitbox>();
			_playerHealth.ModifyShield(-punchHitbox.Player.PunchDmg);
		}
	}
}
