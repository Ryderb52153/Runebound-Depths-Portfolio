using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;
        this.rb = enemy.rb;
        this.anim = enemy.anim;
        this.stats = enemy.stats;
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        float battleSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;

        if(stateMachine.currentState == enemy.BattleMoveState)
            anim.SetFloat("moveSpeedMultiplier", battleSpeedMultiplier);
        else
            anim.SetFloat("moveSpeedMultiplier", enemy.moveAnimSpeedMultiplier);

        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }
}