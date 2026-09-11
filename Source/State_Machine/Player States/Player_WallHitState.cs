using UnityEngine;

public class Player_WallHitState : PlayerState
{
    private bool wallHit = false;

    public Player_WallHitState(Player player, StateMachine stateMachine, string animBoolName) 
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        // Don't call base.Enter() to avoid resetting animations
        // Animations leads from other state.
        input.Disable();
    }

    public override void Update()
    { 
        if (player.CheckForWallWhileThrown() && !wallHit)
        {
            wallHit = true;
            anim.SetBool("WallHit", true);
        }

        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        // Don't call base.Enter() to avoid resetting animations
        // Animations leads from other state.
        wallHit = false;
        triggerCalled = false;
        input.Enable();
        anim.SetBool("WallHit", false);
    }
}