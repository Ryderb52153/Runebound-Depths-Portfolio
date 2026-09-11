using UnityEngine;

public class Enemy_Stationary_BattleState : Enemy_BattleState
{
    public Enemy_Stationary_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        UpdateBattleTimers();
        RefreshTarget();

        if(enemy.PlayerDetected()) 
        {
            AttackConfig selectedAttack = GetAvailableAttack();

            if (selectedAttack != null)
            {
                enemy.HandleFlip(DirectionToPlayer());
                UseAttack(selectedAttack);
                return;
            }
            else
                ReturnToIdle();
        }
    }
}
