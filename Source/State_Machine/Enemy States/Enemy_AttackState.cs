using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    private AttackConfig currentAttack;

    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        currentAttack = enemy.currentAttack;
        anim.SetBool(currentAttack.animationTrigger, true);
        triggerCalled = false;
        SyncAttackSpeed();
        SoundManager.Instance.PlaySFX(currentAttack.soundName);
    }

    public override void Update()
    {
        base.Update();

        if(triggerCalled)
            stateMachine.ChangeState(enemy.BattleState);
    }

    public override void Exit()
    {
        anim.SetBool(currentAttack.animationTrigger, false);
    }
}