using System;
using UnityEngine;

public class SpellProjectile : MonoBehaviour {
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

    public void Init(SpellProjectileData spellProjectileData, float lag) {
        _dmg = spellProjectileData.Dmg;
        _speed = spellProjectileData.Speed;
        _moveDir = spellProjectileData.MoveDir;

        transform.Translate(Vector3.up * _speed * lag);
    }

    public void Update() { Move(); }

    public void Move() {
        // TODO: implement movement types as IMove and iterate thru IMove list, with associated timings even
        transform.Translate(_moveDir * _speed * Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag(TagsLookUp.LookUp[Tags.Wall]) || col.CompareTag(TagsLookUp.LookUp[Tags.Player])) {
            Destroy(gameObject);
        }
    }
}