using UnityEngine;

public class Enemy_Boss_BattleState : Enemy_BattleState
{
    private int currentPhase = 1;

    public Enemy_Boss_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        
    }

    protected override AttackConfig GetAvailableAttack()
    {
        if(enemy.attacks.Count == 0) return null;

        //goblinEnemy.SetPhaseByHealth();

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

    protected override bool IsAttackValid(float distance, AttackConfig attack)
    {
        if (!attack.IsReady())
            return false;

        if (!attack.IsInRange(distance))
        {
            outOfRange = attack.maxRange;
            return false;
        }

        if (currentPhase < attack.phaseRequired)
            return false;

        return true;
    }

    protected override void Rest()
    {
        stateMachine.ChangeState(enemy.StunnedState);
        attacksUsedBeforeRest = Random.Range(8, 12);
    }
}