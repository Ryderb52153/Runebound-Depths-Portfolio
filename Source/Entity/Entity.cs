using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlipped;

    protected StateMachine stateMachine;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    public bool platformDetected { get; private set; }

    private bool _facingRight = true;
    public int facingDirection { get; private set; } = 1;
    public LayerMask groundMask;

    [Header("Collision Detection")]
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] protected float _wallCheckDistance = 0.5f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] protected Transform _primaryWallCheck;
    [SerializeField] private Transform _secondaryWallCheck;

    [Header("Sound Details")]
    public SoundName hurtSound;
    public SoundName deathSound;
    public SoundName footstepsSound;

    public bool isKnocked { get; private set; }
    private Coroutine knockbackCo;
    private Coroutine slowDownCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        HandleCollissionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void EntityDeath()
    {

    }

    public virtual void SlowDownEntity(float duration, float slowMultiplier, bool canOverrideSlow = false)
    {
        if (slowDownCo != null)
        {
            if(canOverrideSlow)
                StopCoroutine(slowDownCo);
            else
                return;
        }

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    public virtual void StopSlowDown()
    {
        slowDownCo = null;
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null;
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if(knockbackCo != null)
        {
            StopCoroutine(knockbackCo);
        }

        knockbackCo = StartCoroutine(KnockBackCo(knockback, duration));
    }

    private IEnumerator KnockBackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if(isKnocked) return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void Flip()
    { 
        transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
        facingDirection = facingDirection * -1;
        OnFlipped?.Invoke();
    }

    public void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !_facingRight)
        {
            Flip();
        }
        else if (xVelocity < 0 && _facingRight)
        {
            Flip();
        }
    }

    private void HandleCollissionDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(_groundCheck.position, Vector2.down, _groundCheckDistance, groundMask);
        groundDetected = hit;

        if(hit.collider != null && hit.collider.CompareTag("Platform"))
            platformDetected = true;
        else
            platformDetected = false;

        if (_secondaryWallCheck != null)
        {
            wallDetected = Physics2D.Raycast(_primaryWallCheck.position, Vector2.right * facingDirection, _wallCheckDistance, groundMask)
                        || Physics2D.Raycast(_secondaryWallCheck.position, Vector2.right * facingDirection, _wallCheckDistance, groundMask);
        }
        else
        {
            wallDetected = Physics2D.Raycast(_primaryWallCheck.position, Vector2.right * facingDirection, _wallCheckDistance, groundMask);
        }
    }

    protected virtual void OnEnable()
    {
        _facingRight = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        facingDirection = 1;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(_groundCheck.position, _groundCheck.position + new Vector3(0, -_groundCheckDistance));
        Gizmos.DrawLine(_primaryWallCheck.position, _primaryWallCheck.position + new Vector3(_wallCheckDistance * facingDirection, 0));

        if (_secondaryWallCheck != null)
        {
            Gizmos.DrawLine(_secondaryWallCheck.position, _secondaryWallCheck.position + new Vector3(_wallCheckDistance * facingDirection, 0));
        }
    }
}