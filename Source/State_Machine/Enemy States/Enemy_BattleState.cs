using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    protected Transform player;
    protected float outOfRange = 2;
    protected int attacksUsedBeforeRest = 4;

    private Transform lastTarget;
    private float lastTimeSeenPlayer;
    private float retreatCooldown;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        UpdateBattleTimers();
        RefreshTarget();
    }

    // Prioritise attacking an Echo over the player. Falls back to the player
    // when no echo is active (e.g. the echo expired or was destroyed).
    protected void RefreshTarget()
    {
        Transform target = enemy.GetCurrentTarget();

        if (target != null)
            player = target;
    }

    public override void Update()
    {
        if (enemy.isKnocked)
            return;

        base.Update();

        UpdateBattleTimers();
        RefreshTarget();

        if (BattleTimeIsOver())
        {
            ReturnToIdle();
            return;
        }

        if (AllAttacksUsed())
        {
            Rest();
            return;
        }

        if (PlayerIsTooClose())
        {
            RetreatBack();
            return;
        }

        AttackConfig selectedAttack = GetAvailableAttack();

        if (selectedAttack != null)
        {
            enemy.HandleFlip(DirectionToPlayer());
            UseAttack(selectedAttack);
            attacksUsedBeforeRest--;
            return;
        }
        else
        {
            ChasePlayer();
        }
    }

    protected void UseAttack(AttackConfig attack)
    {
        attack.MarkUsed();
        enemy.currentAttack = attack;
        stateMachine.ChangeState(enemy.AttackState);
    }

    protected virtual void Rest()
    {
        // go to idle for a quick breather, then return to battle
        attacksUsedBeforeRest = Random.Range(1, 3);
    }

    protected virtual AttackConfig GetAvailableAttack()
    {
        if (enemy.attacks.Count == 0) return null;

        float distance = DistanceToPlayer();
        AttackConfig bestAttack = null;

        foreach (AttackConfig attack in enemy.attacks)
        {
            if (!IsAttackValid(distance, attack))
                continue;

            if (bestAttack == null || attack.priority > bestAttack.priority)
                bestAttack = attack;
        }

        return bestAttack;
    }

    protected virtual bool IsAttackValid(float distance, AttackConfig attack)
    {
        if (!attack.IsReady())
            return false;

        if (!attack.IsInRange(distance))
        {
            outOfRange = attack.maxRange;
            return false;
        }

        return true;
    }

    private bool AllAttacksUsed() => attacksUsedBeforeRest <= 0;

    private bool BattleTimeIsOver() => Time.time >= lastTimeSeenPlayer + enemy.battleTimeDuration;

    private bool PlayerIsTooClose() => DistanceToPlayer() < enemy.minRetreatDistance;

    private void ChasePlayer()
    {
        enemy.BattleMoveState.desiredRange = outOfRange;
        enemy.BattleMoveState.player = player;
        stateMachine.ChangeState(enemy.BattleMoveState);
    }

    protected void ReturnToIdle()
    {
        stateMachine.ChangeState(enemy.IdleState);
    }

    protected void RetreatBack()
    {
        if(retreatCooldown > 0)
            return;

        rb.linearVelocity = new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer(), enemy.retreatVelocity.y);
        enemy.HandleFlip(DirectionToPlayer());
        retreatCooldown = .5f;
    }

    protected void UpdateTargetIfNeeded()
    {
        if(enemy.PlayerDetected() == false)
            return;

        Transform newTarget = enemy.PlayerDetected().transform;

        if(newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    protected void UpdateBattleTimers()
    {
        if (retreatCooldown > 0)
            retreatCooldown -= Time.deltaTime;

        lastTimeSeenPlayer = Time.time;
    }

    protected float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Vector2.Distance(player.position, enemy.transform.position);
    }

    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}

[System.Serializable]
public class AttackConfig
{
    public string attackName;
    public SoundName soundName;
    public string animationTrigger;
    public float cooldown;
    public float minRange;
    public float maxRange;
    public int priority;
    public int phaseRequired = 1;
    public GameObject prefab;

    [HideInInspector] public float lastUsedTime = -999f;

    public bool IsReady() => Time.time >= lastUsedTime + cooldown;
    public bool IsInRange(float distance) => distance >= minRange && distance <= maxRange;
    public void MarkUsed() => lastUsedTime = Time.time;
}
