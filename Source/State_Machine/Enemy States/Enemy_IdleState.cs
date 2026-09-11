using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    override public void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            if(enemy.MoveState != null)
                stateMachine.ChangeState(enemy.MoveState);
        }
    }
}