using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    private Enemy_IdleState idleState;
    private Enemy_MoveState moveState;
    private Enemy_AttackState attackState;
    private Enemy_BattleState battleState;
    private Enemy_DeadState deadState;
    private Enemy_StunnedState stunnedState;
    private Enemy_Battle_MoveState battleMoveState;

    public Enemy_IdleState IdleState { get => idleState; set => idleState = value; }
    public Enemy_MoveState MoveState { get => moveState; set => moveState = value; }
    public Enemy_AttackState AttackState { get => attackState; set => attackState = value; }
    public Enemy_BattleState BattleState { get => battleState; set => battleState = value; }
    public Enemy_DeadState DeadState { get => deadState; set => deadState = value; }
    public Enemy_StunnedState StunnedState { get => stunnedState; set => stunnedState = value; }
    public Enemy_Battle_MoveState BattleMoveState { get => battleMoveState; set => battleMoveState = value; }

    [Header("Battle Details")]
    public List<AttackConfig> attacks = new List<AttackConfig>();
    [HideInInspector] public AttackConfig currentAttack;
    public float battleMoveSpeed = 3;
    public float meleeAttackDistance = 2;
    public float rangedAttackDistance = 5;
    public float battleTimeDuration = 5;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;

    [Header("Stunned State Details")]
    public bool canBeStunned = false;
    public float stunnedDuration = 1;
    public Vector2 stunnedVelocity = new Vector2(7,7);

    [Header("Movement Details")]
    public float idleTime = 2;
    public float moveSpeed = 1.4f;
    [Range(0,2)]
    public float moveAnimSpeedMultiplier = 1f;

    [Header("Player Detection")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float playerCheckY;
    [SerializeField] private float playerCheckRadius = 10;
    [SerializeField] private float detectionBehindDelay = 1;
    [SerializeField] private int statsWorth = 1;

    public float detectionDelay { get; private set; }
    public float detectionDelayRemaining { get; set; }

    public float activeSlowMultiplier { get; private set; } = 1;
    public Entity_Stats stats { get; private set; }
    public Enemy_Health health { get; private set; }


    public event Action<Enemy> OnDeath;
    public static event Action<Vector3> OnEnemyFootstep;

    public static void RaiseFootstep(Vector3 position) => OnEnemyFootstep?.Invoke(position);

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;
    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;
    public void EnableCounterWindow(bool enable) => canBeStunned = enable;

    protected Transform player;
    private int playerLayer;
    private int echoLayer;
    private Coroutine dying;

    protected override void Awake()
    {
        base.Awake();
        playerLayer = LayerMask.NameToLayer("Player");
        echoLayer = LayerMask.NameToLayer("Echo Player");
        stats = GetComponent<Entity_Stats>();
        health = GetComponent<Enemy_Health>();
        detectionDelay = detectionBehindDelay;
        detectionDelayRemaining = detectionDelay;
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        activeSlowMultiplier = 1 - slowMultiplier;
        anim.speed *= activeSlowMultiplier;

        yield return new WaitForSeconds(duration);
        StopSlowDown();
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1;
        anim.speed = 1;
        base.StopSlowDown();
    }

    public void TryEnterBattleState(Transform player)
    {
        if(stateMachine.currentState == BattleState || stateMachine.currentState == AttackState) 
            return;

        this.player = player;
        stateMachine.ChangeState(BattleState);
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        stateMachine.ChangeState(DeadState);
        stateMachine.SetStateChanging(false);
        OnDeath?.Invoke(this);
        GivePlayerRewards();

        if (dying == null)
            dying = StartCoroutine(DespawnEnemyCo());
        else
            return;
    }

    private void GivePlayerRewards()
    {
        int rand = UnityEngine.Random.Range(0, 2);

        if (rand == 0)
            Player.Instance.stats.AddStatPoint(statsWorth);
        else
            GoldManager.Instance.AddGold(statsWorth * 10);

        if (!ItemDrop_Manager.ChanceToDrop())
            return;

        ItemDrop_Manager.SpawnRandomItemPU(transform.position);
    }

    private IEnumerator DespawnEnemyCo()
    {
        yield return new WaitForSeconds(5f);
        stateMachine.SetStateChanging(true);
        stateMachine.ChangeState(IdleState);
        dying = null;
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public Transform GetPlayer()
    {
        if(player == null)
        {
            if (Player.Instance != null)
                player = Player.Instance.transform;
        }

        return player;
    }

    // Returns the highest priority target for the enemy: the closest active
    // Time Echo within detection range if one exists, otherwise the player.
    public Transform GetCurrentTarget()
    {
        Transform echo = GetClosestEcho();

        if (echo != null)
            return echo;

        return GetPlayer();
    }

    private Transform GetClosestEcho()
    {
        var echoes = Skill_Object_TimeEcho.ActiveEchoes;

        Transform closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < echoes.Count; i++)
        {
            Skill_Object_TimeEcho echo = echoes[i];

            if (echo == null || !echo.isActiveAndEnabled)
                continue;

            float distance = Vector2.Distance(echo.transform.position, transform.position);

            if (distance <= playerCheckRadius && distance < closestDistance)
            {
                closestDistance = distance;
                closest = echo.transform;
            }
        }

        return closest;
    }

    // The enemy detects both the Player and a Time Echo (the echo lives on its
    // own "Echo Player" layer, which is OR'd in so detection works regardless
    // of how each enemy's playerMask is configured).
    private LayerMask GetDetectionMask() => playerMask | groundMask | (1 << echoLayer);

    private bool IsTargetLayer(int layer) => layer == playerLayer || layer == echoLayer;

    public RaycastHit2D PlayerDetected()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + playerCheckY);

        RaycastHit2D hit = 
            Physics2D.Raycast(rayOrigin, Vector2.right * facingDirection , playerCheckRadius, GetDetectionMask());

        if (hit.collider == null || !IsTargetLayer(hit.collider.gameObject.layer))
            return default;

        return hit;
    }

    public RaycastHit2D PlayerDetectedBehind()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + playerCheckY);

        RaycastHit2D playerBehind =
            Physics2D.Raycast(rayOrigin, Vector2.right * -facingDirection, playerCheckRadius, GetDetectionMask());

        if(playerBehind.collider == null || !IsTargetLayer(playerBehind.collider.gameObject.layer))
            return default;

        return playerBehind;
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(IdleState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Player.OnPlayerDeath += HandlePlayerDeath;
        //stateMachine.ChangeState(idleState);
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Vector3 startPos = transform.position;
        startPos.y += playerCheckY;
        Vector3 direction = new Vector2(1, 0) * facingDirection;
        direction.Normalize();
        Vector3 endPos = startPos + direction * playerCheckRadius;

        // PlayerDetected raycast visualization
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(endPos, 0.2f);

        // Existing gizmos
        Vector3 horizontalDirection = new Vector3(transform.position.x, transform.position.y + playerCheckY, 0) * facingDirection;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(horizontalDirection, new Vector3(transform.position.x + (facingDirection * playerCheckRadius), horizontalDirection.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(horizontalDirection, new Vector3(transform.position.x + (facingDirection * meleeAttackDistance), horizontalDirection.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(horizontalDirection, new Vector3(transform.position.x + (facingDirection * minRetreatDistance), horizontalDirection.y));
        Gizmos.color = Color.purple;
        Gizmos.DrawLine(horizontalDirection + new Vector3(0,.5f,0), new Vector3(transform.position.x + (facingDirection * rangedAttackDistance), horizontalDirection.y + .5f));
    }
}
