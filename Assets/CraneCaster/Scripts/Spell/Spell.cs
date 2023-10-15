using System;
using Photon.Pun;
using UnityEngine;

public class Spell : MonoBehaviour {
    public int Dmg => _dmg;
    [SerializeField] int _dmg;

    // Velocity - consider combining
    public float Speed => _speed;
    [SerializeField] float _speed;
    public Vector2 MoveDir => _moveDir;
    [SerializeField] Vector2 _moveDir;

    public void Start() {
        // Spell lifespan
        Destroy(gameObject, 10.0f);
    }

    public void Init(SpellData spellData, float lag) {
        _dmg = spellData.Dmg;
        _speed = spellData.Speed;
        _moveDir = spellData.MoveDir;

        transform.Translate(Vector3.up * _speed * lag);
    }

    public void Update() { Move(); }

    public void Move() {
        // TODO: implement movement types as IMove and iterate thru IMove list, with associated timings even
        transform.Translate(_moveDir * _speed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D col) {
        Destroy(gameObject);
    }
}