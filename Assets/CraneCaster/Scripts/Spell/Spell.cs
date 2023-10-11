using System;
using Photon.Pun;
using UnityEngine;

public class Spell : MonoBehaviour, IPunInstantiateMagicCallback {
	public int Dmg => _dmg;
	[SerializeField] int _dmg;
	
	public float Speed => _speed;
	[SerializeField] float _speed;

	void Awake() {
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom) enabled = false;
	}

	public void Init(SpellData spellData) {
		_dmg = spellData.Dmg;
		_speed = spellData.Speed;
	}
	
	public void OnPhotonInstantiate(PhotonMessageInfo info) { Init((SpellData) info.photonView.InstantiationData[0]); }

	public void Update() {
		Move();
	}

	public void Move() {
		// TODO: implement movement types as IMove and iterate thru IMove list, with associated timings even
		transform.Translate(Vector3.up * _speed * Time.deltaTime);
	}
}
