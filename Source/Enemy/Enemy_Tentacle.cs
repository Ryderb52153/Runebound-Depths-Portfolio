using System.Collections;
using UnityEngine;

public class Enemy_Tentacle : Enemy, ICounterable, Enemy_Minion
{
    [SerializeField] private float minionDuration;

    public bool CanBeCountered { get => canBeStunned; }

    private Enemy_SpawnState spawnState;

    private float minionTimer;
    
    private bool isActive = false;

    protected override void Awake()
    {
        base.Awake();

        spawnState = new Enemy_SpawnState(this, stateMachine, "spawn");
        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_Stationary_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "dead");
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    protected override void Update()
    {
        base.Update();

        HandleDuration();
    }

    public void HandleCounterAttack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }

    public void HandleDuration()
    {
        if (!isActive || health.isDead)
            return;

        if (stateMachine.currentState != IdleState)
            return;

        minionTimer -= Time.deltaTime;

        if (minionTimer <= 0f)
            StartCoroutine(ReverseSpawnCo());
    }

    private IEnumerator ReverseSpawnCo()
    {
        isActive = false;
        anim.SetBool("idle", false);
        anim.SetBool("reverseSpawn", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("reverseSpawn", false);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public void SetUpMinion()
    {
        minionTimer = minionDuration;
        isActive = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        stateMachine.Initialize(spawnState);
    }
}