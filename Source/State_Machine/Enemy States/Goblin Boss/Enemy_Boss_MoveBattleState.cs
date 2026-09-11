using UnityEngine;

public class Enemy_Boss_MoveBattleState : Enemy_Battle_MoveState
{
    public Enemy_Boss_MoveBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
    }

    public override void Update()
    {
        if (DistanceToPlayer() <= desiredRange)
            anim.SetBool("moveEnd", true);
        else
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocity.y);

        if (triggerCalled)
        {
            triggerCalled = false;
            stateMachine.ChangeState(enemy.BattleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("moveEnd", false);
    }
}