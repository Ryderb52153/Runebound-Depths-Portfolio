using UnityEngine;

public class Player_ClimbState : PlayerState
{

    private float climbSoundTimer = 0f;

    public Player_ClimbState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();

        if (player.currentLadder != null)
        {
            float targetX = player.currentLadder.bounds.center.x;
            float newX = Mathf.Lerp(player.transform.position.x, targetX, Time.deltaTime * 10f);
            player.transform.position = new Vector2(newX, player.transform.position.y);

            // Top of ladder check
            float topBound = player.currentLadder.bounds.max.y;
            if (player.transform.position.y > topBound - 0.5f && player.movementInput.y > 0)
            {
                 // Small boost to get over the top
                player.SetVelocity(rb.linearVelocity.x, player.climbSpeed);
            }
        }

        if(player.movementInput.y == 0)
        {
            stateMachine.ChangeState(player.climbIdleState);
        }
        else
        {
            float climbVelocity = player.climbSpeed * player.movementInput.y;
            player.SetVelocity(0, climbVelocity);
            anim.speed = Mathf.Abs(player.movementInput.y);
            HandleLadderSound();
        }

        if (input.Player.Jump.WasPressedThisFrame())
        {
            if (player.movementInput.x != 0)
            {
                player.SetVelocity(player.movementInput.x * player.moveSpeed, player.jumpForce);
            }
            stateMachine.ChangeState(player.jumpState);
        }

        if (player.groundDetected && player.movementInput.y < -0.1f)
            stateMachine.ChangeState(player.idleState);
    }

    private void HandleLadderSound()
    {
        climbSoundTimer += Time.deltaTime;
        if (climbSoundTimer >= 0.5f)
        {
            SoundManager.Instance.PlaySFX(SoundName.Chain_Climb);
            climbSoundTimer = 0f;
        }
    }

    override public void Exit()
    {
        base.Exit();
        rb.gravityScale = 3.5f;
        anim.speed = 1f;
    }
}
