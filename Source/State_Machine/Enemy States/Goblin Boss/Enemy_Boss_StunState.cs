using UnityEngine;

public class Enemy_Boss_StunState : Enemy_StunnedState
{
    public Enemy_Boss_StunState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.stunnedDuration;
        triggerCalled = false;
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if(triggerCalled)
        {
            stateMachine.ChangeState(enemy.BattleState);
            return;
        }

        if (stateTimer <= 0)
            anim.SetBool("stunOut", true);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("stunOut", false);
    }
}