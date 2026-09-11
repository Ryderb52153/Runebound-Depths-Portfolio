using UnityEngine;

public class Player_Wall_JumpState : PlayerState
{
    public Player_Wall_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        //player.SetVelocity(player.wallJumpForce.x * -player.facingDirection, player.wallJumpForce.y); 
        player.SetVelocity(player.wallJumpForce.x * -player.facingDirection, player.jumpState.adjustedJumpForce);
    }

    override public void Update()
    {
        base.Update();

        if(rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}