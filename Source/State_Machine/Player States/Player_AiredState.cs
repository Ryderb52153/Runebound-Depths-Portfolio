using UnityEngine;

public class Player_AiredState : PlayerState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        if(player.movementInput.x != 0)
            player.SetVelocity(player.movementInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);

        if(input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpAttackState);

       
        // check if we recently grabbed a ledge


        if (player.CheckForLedge())
            stateMachine.ChangeState(player.ledgeGrabState);
        else if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
