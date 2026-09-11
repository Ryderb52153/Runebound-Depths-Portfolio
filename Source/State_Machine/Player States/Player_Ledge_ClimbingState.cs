using UnityEngine;

public class Player_Ledge_ClimbingState : PlayerState
{
    public Vector2 LedgePoint { get; set; }
    private Vector2 climbTarget;
    private bool isClimbing;
    private bool isLanding;
    private float savedGravity;

    public Player_Ledge_ClimbingState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        isClimbing = true;
        isLanding = false;
        savedGravity = rb.gravityScale;
        player.SetVelocity(0, 0);
        rb.gravityScale = 0f;
        rb.simulated = false;
        Vector2 standOffset = new Vector2(.5f * player.facingDirection, 1.8f);
        climbTarget = LedgePoint + standOffset;
    }

    public override void Update()
    {
        if (triggerCalled && isLanding)
        {
            stateMachine.ChangeState(player.idleState);
            triggerCalled = false;
        }
        else if (triggerCalled && isClimbing)
        {
            MovePlayerToClimbSpot();
            triggerCalled = false;
        }
    }

    private void MovePlayerToClimbSpot()
    {
        anim.SetBool("ledgeLand", true);
        isLanding = true;
        player.transform.position = climbTarget;
        rb.simulated = true;
        rb.gravityScale = savedGravity;
        isClimbing = false;
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("ledgeLand", false);
        isLanding = false;
    }
}