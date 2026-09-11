using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public float adjustedJumpForce;
    private float jumpTimeCounter;
    private float maxJumpTime = .3f; // Maximum time the player can hold jump
    private float jumpHoldForce = 15f; // Additional force applied while holding jump
    private bool isJumpHeld;

    public override void Enter()
    {
        base.Enter();
        jumpTimeCounter = 0f;
        isJumpHeld = true;

        // Apply initial jump force
        if(rb.linearVelocity.x == 0)
        {
            player.SetVelocity(0, player.idleJumpForce);
            adjustedJumpForce = player.idleJumpForce;
            return;
        }

        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        adjustedJumpForce = player.jumpForce * (1f + horizontalSpeed * 0.05f);

        player.SetVelocity(rb.linearVelocity.x, adjustedJumpForce);
    }

    public override void Update()
    {
        base.Update();

        // Check if jump button is still held
        if(input.Player.Jump.IsPressed() && isJumpHeld && jumpTimeCounter < maxJumpTime)
        {
            jumpTimeCounter += Time.deltaTime;
            // Continue applying upward force while jump is held
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + jumpHoldForce * Time.deltaTime);
        }
        else
        {
            // Jump button released or max time reached
            if(isJumpHeld)
            {
                isJumpHeld = false;
                // Optional: reduce upward velocity for shorter jumps
                if(rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                }
            }
        }

        if(rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}