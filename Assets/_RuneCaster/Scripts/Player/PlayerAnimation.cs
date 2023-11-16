using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSprite;
    
    private Animator _animator;
    private Vector2 _lastPos;

    private const string SpeedParameter = "speed";
    private static readonly int SpeedHash = Animator.StringToHash(SpeedParameter);

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _lastPos = transform.position;
    }

    float _syncTime;
    Vector2 _movementVector = Vector2.zero;
    public void Update() {
        if (Time.time > _syncTime) { // slight delay on updating movement vector to prevent stuttering on other clients
            _movementVector = (Vector2) transform.position - _lastPos;
            _animator.SetFloat(SpeedHash, _movementVector.magnitude);

            _syncTime = Time.time + 0.02f; // some value that plays well with network sync rate for player pos - idk how well this works on slow connections
            _lastPos = transform.position;
        }

        if (Mathf.Abs(_movementVector.x) > 0.02f)
        {
            playerSprite.flipX = _movementVector.x > 0;
        }
    }
}
