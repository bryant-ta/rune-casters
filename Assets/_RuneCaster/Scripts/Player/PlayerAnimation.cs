using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSprite;
    
    private Rigidbody2D _rb;
    private Animator _animator;

    private const string SpeedParameter = "speed";
    private static readonly int SpeedHash = Animator.StringToHash(SpeedParameter);

    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    Vector2 _lastPos;
    public void Update() {
        Vector2 movementVector = (Vector2) transform.position - _lastPos;
        float movementDiff = movementVector.magnitude * Math.Sign(movementVector.x);
        print(movementDiff);
        _animator.SetFloat(SpeedHash, movementVector.magnitude);
        if (Mathf.Abs(movementDiff) > 0.05f)
        {
            playerSprite.flipX = movementDiff > 0;
        }

        _lastPos = transform.position;
    }
}
