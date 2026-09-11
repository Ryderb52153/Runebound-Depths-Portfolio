using UnityEngine;

public class Enemy_SpawnState : EnemyState
{
    public Enemy_SpawnState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if(triggerCalled)
            stateMachine.ChangeState(enemy.IdleState);
    }
}