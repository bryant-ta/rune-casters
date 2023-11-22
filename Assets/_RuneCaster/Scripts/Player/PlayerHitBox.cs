using Game;
using Photon.Pun;
using UnityEngine;

// needs to be separate script to attach to own collider separate from player pickup collider
public class PlayerHitBox : MonoBehaviour {
	[SerializeField] Player _player;
	[SerializeField] PlayerHealth _playerHealth;

	public void OnTriggerEnter2D(Collider2D col) {
		if (!PhotonNetwork.IsMasterClient) return;

		if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Spell])) {
			SpellProjectile spellProjectile = col.gameObject.GetComponent<SpellProjectile>();
			_playerHealth.ModifyHp(-spellProjectile.Dmg);
		}
		if (col.gameObject.CompareTag(TagsLookUp.LookUp[Tags.Punch])) {
			PunchHitbox punchHitbox = col.gameObject.GetComponent<PunchHitbox>();
			_playerHealth.ModifyHp(-punchHitbox.Player.PunchDmg);
			_player.StunnedTimer.Start();
		}
	}
}
