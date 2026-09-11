using UnityEngine;

public class VampireBat : MonoBehaviour
{
    private enum BatPhase { Flying, Attacking, TurningToBlood, ReturningAsBlood, Complete }
    private BatPhase currentPhase;

    [Header("Movement Settings")]
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private float returnSpeed = 12f;
    [SerializeField] private float maxLifetime = 5f;

    [Header("Damage Settings")]
    [SerializeField] private int damage = 15;

    private Attack_Data attackData;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Transform vampireTransform;
    private Enemy_Vampire _vampire;
    private float lifetime;
    private bool hasHitPlayer;
    private int playerLayer;

    private void Awake()
    {
        if(anim == null)
            anim = GetComponentInChildren<Animator>();

        if(rb == null)
            rb = GetComponent<Rigidbody2D>();

        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnEnable()
    {
        currentPhase = BatPhase.Flying;
        lifetime = 0f;
        hasHitPlayer = false;
        anim.SetBool("fly", true);
    }

    public void Initialize(Vector2 targetPos, Transform vampire)
    {
        targetPosition = targetPos;
        vampireTransform = vampire;

        if (_vampire == null)
        {
            _vampire = vampire.GetComponent<Enemy_Vampire>();
            IntializeAttack();
        }


        // Face the direction of movement
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;

        if (lifetime >= maxLifetime && currentPhase == BatPhase.Flying)
        {
            DespawnBat();
            return;
        }

        switch (currentPhase)
        {
            case BatPhase.Flying:
                UpdateFlyingPhase();
                break;
            case BatPhase.Attacking:
                UpdateAttackingPhase();
                break;
            case BatPhase.TurningToBlood:
                UpdateTurningToBloodPhase();
                break;
            case BatPhase.ReturningAsBlood:
                UpdateReturningAsBloodPhase();
                break;
            case BatPhase.Complete:
                DespawnBat();
                break;
        }
    }

    private void UpdateFlyingPhase()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * flySpeed;

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.5f)
        {
            // Missed the player, despawn or return
            StartReturning(false);
        }
    }

    private void UpdateAttackingPhase()
    {
        rb.linearVelocity = Vector2.zero;

        if (CheckAnimationFinished("Bat_Basic_Attack"))
            StartTurningToBlood();
    }

    private void UpdateTurningToBloodPhase()
    {
        rb.linearVelocity = Vector2.zero;

        if (CheckAnimationFinished("Bat_Turn_To_Blood"))
            StartReturningAsBlood();
    }

    private void UpdateReturningAsBloodPhase()
    {
        if (vampireTransform == null)
        {
            DespawnBat();
            return;
        }

        Vector2 direction = ((Vector2)vampireTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * returnSpeed;

        float distanceToVampire = Vector2.Distance(transform.position, vampireTransform.position);
        if (distanceToVampire < 1f)
        {
            if (hasHitPlayer)
                _vampire.health.IncreaseHealth(damage);

            currentPhase = BatPhase.Complete;
        }
    }

    private void StartAttacking()
    {
        currentPhase = BatPhase.Attacking;
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("fly", false);
        anim.SetBool("attack", true);
        hasHitPlayer = true;
    }

    private void StartTurningToBlood()
    {
        currentPhase = BatPhase.TurningToBlood;
        anim.SetBool("attack", false);
        anim.SetBool("intoBlood", true);
    }

    private void StartReturningAsBlood()
    {
        currentPhase = BatPhase.ReturningAsBlood;
        anim.SetBool("moveBack", true);

        if (vampireTransform != null)
        {
            Vector2 direction = ((Vector2)vampireTransform.position - (Vector2)transform.position).normalized;
            if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void StartReturning(bool hitPlayer)
    {
        hasHitPlayer = hitPlayer;

        if (hitPlayer)
            StartAttacking();
        else
            StartTurningToBlood();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentPhase != BatPhase.Flying)
            return;

        // Check if hit player
        if (collision.gameObject.layer == playerLayer)
        {
            DealDamageToPlayer();
            StartReturning(true);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            DespawnBat();
        }
    }

    private void DealDamageToPlayer()
    {
        Player.Instance.health.TakeDamage(attackData, null);
    }

    private bool CheckAnimationFinished(string animName)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= 0.95f;
    }

    private void IntializeAttack()
    {
        attackData = new Attack_Data(_vampire.stats, new Damage_Scale_Data());
        attackData.physicalDamage = damage;
        attackData.elementalDamage = 0;
    }

    private void DespawnBat()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnDisable()
    {
        anim.SetBool("fly", false);
        anim.SetBool("moveBack", false);
        rb.linearVelocity = Vector2.zero;
    }
}