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

    public void Update() {
        Vector2 movementVector = (Vector2) transform.position - _lastPos;
        _animator.SetFloat(SpeedHash, movementVector.magnitude);
        if (Mathf.Abs(movementVector.x) > 0.02f)
        {
            playerSprite.flipX = movementVector.x > 0;
        }

        _lastPos = transform.position;
    }
}
