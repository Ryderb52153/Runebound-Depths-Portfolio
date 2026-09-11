using UnityEngine;

public class Player_Jump_AttackState : PlayerState
{
    private bool touchedGround;

    public Player_Jump_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        touchedGround = false;

        player.SetVelocity(player.jumpAttackVelocity.x * player.facingDirection, player.jumpAttackVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (player.groundDetected && touchedGround == false)
        {
            touchedGround = true;
            anim.SetTrigger("jumpAttackTrigger");
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        if(triggerCalled && player.groundDetected)
            stateMachine.ChangeState(player.idleState);
    }
}