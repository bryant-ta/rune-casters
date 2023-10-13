using System;
using Photon.Pun;
using UnityEngine;

public class Spell : MonoBehaviour {
	public int Dmg => _dmg;
	[SerializeField] int _dmg;
	
	public float Speed => _speed;
	[SerializeField] float _speed;
	
	
	
	
	
	
	
	
	public void Start()
	{
		Destroy(gameObject, 10.0f);
	}

	public void OnTriggerEnter2D(Collider2D col) {
		// Destroy(gameObject);
	}

	public void Init(SpellData spellData, float lag) {
		_dmg = spellData.Dmg;
		_speed = spellData.Speed;
		
		transform.Translate(Vector3.up * _speed * lag);
	}

	public void Update() {
		Move();
	}

	public void Move() {
		// TODO: implement movement types as IMove and iterate thru IMove list, with associated timings even
		transform.Translate(Vector3.up * _speed * Time.deltaTime);
	}
}
