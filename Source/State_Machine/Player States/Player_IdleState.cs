using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (player.movementInput.x == player.facingDirection && player.wallDetected)
            return;

        if (player.movementInput.x != 0)
            stateMachine.ChangeState(player.moveState);
        else if (player.movementInput == Vector2.down && player.platformDetected)
            player.DropDownPlatform();
    }
}