using UnityEngine;

public class Player_Climb_IdleState : PlayerState
{
    public Player_Climb_IdleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 0;
        player.SetVelocity(0,0);
        SoundManager.Instance.PlaySFX(SoundName.Chain_Climb);
    }

    public override void Update()
    {
        base.Update();

        if (player.currentLadder != null)
        {
            float targetX = player.currentLadder.bounds.center.x;
            float newX = Mathf.Lerp(player.transform.position.x, targetX, Time.deltaTime * 10f);
            player.transform.position = new Vector2(newX, player.transform.position.y);
        }

        if(player.movementInput.y != 0)
            stateMachine.ChangeState(player.climbState);

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

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = 3.5f;
    }
}
