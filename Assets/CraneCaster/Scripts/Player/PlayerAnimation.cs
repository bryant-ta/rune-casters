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
    
    public void Update()
    {
        _animator.SetFloat(SpeedHash, _rb.velocity.magnitude);
        float xVelocity = _rb.velocity.x;
        if (!Mathf.Approximately(xVelocity, 0))
        {
            playerSprite.flipX = xVelocity > 0;
        }
    }
}
