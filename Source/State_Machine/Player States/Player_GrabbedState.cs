using UnityEngine;

public class Player_GrabbedState : PlayerState
{
    public Player_GrabbedState(Player player, StateMachine stateMachine, string animBoolName) 
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, 0);
        input.Disable();
    }

    public override void Update()
    {
        base.Update();

        // Player is controlled by the vampire's grab state
        // Do nothing, just wait for release
    }

    public override void Exit()
    {
        base.Exit();
        input.Enable();
    }
}