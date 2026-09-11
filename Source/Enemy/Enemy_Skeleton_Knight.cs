using System.Collections;
using UnityEngine;

public class Enemy_Skeleton_Knight : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }

    protected override void Awake()
    {
        base.Awake();

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_BattleState(this, stateMachine, "battle");
        BattleMoveState = new Enemy_Battle_MoveState(this, stateMachine, "move");
        DeadState = new Enemy_DeadState(this, stateMachine, "dead");
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    private void EntrySequence()
    {
        StartCoroutine(StartEntrySequence());
    }

    private IEnumerator StartEntrySequence()
    {
        anim.SetBool("entry", true);
        rb.simulated = false; 

        // Wait for the entry animation to complete
        yield return new WaitForSeconds(1.938f);

        anim.SetBool("entry", false);
        anim.SetBool("idle", true);
        rb.simulated = true;

        InitializeBattle();
    }

    private void InitializeBattle()
    {
        Transform player = GetPlayer();

        if(player != null)
            stateMachine.Initialize(BattleState);
    }

    public void HandleCounterAttack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EntrySequence();
    }
}
