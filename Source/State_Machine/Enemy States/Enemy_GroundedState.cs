using UnityEngine;

public class Enemy_GroundedState : EnemyState
{
    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected())
            stateMachine.ChangeState(enemy.BattleState);

        if (enemy.PlayerDetectedBehind())
            HandlePlayerBehind();
    }


    private void HandlePlayerBehind()
    {
        enemy.detectionDelayRemaining -= Time.deltaTime;

        if (enemy.detectionDelayRemaining <= 0f)
        {
            stateMachine.ChangeState(enemy.BattleState);
            enemy.Flip();
            enemy.detectionDelayRemaining = enemy.detectionDelay;
        }
    }
}
