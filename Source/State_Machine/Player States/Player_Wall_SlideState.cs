using UnityEngine;

public class Player_Wall_SlideState : PlayerState
{
    public Player_Wall_SlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();
        HandleWallSlide();

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.wallJumpState);

        if(player.wallDetected ==false)
            stateMachine.ChangeState(player.fallState);

        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);

            if(player.facingDirection != player.movementInput.x)
                player.Flip();
        }
    }

    private void HandleWallSlide()
    {
        if (player.movementInput.y < 0)
        {
            player.SetVelocity(player.movementInput.x, rb.linearVelocity.y);
            anim.SetBool("wallSlideAccelerate", true);
        }
        else
        {
            player.SetVelocity(player.movementInput.x, rb.linearVelocity.y * player.wallSlideMultiplier);
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("wallSlideAccelerate", false);
    }
}
