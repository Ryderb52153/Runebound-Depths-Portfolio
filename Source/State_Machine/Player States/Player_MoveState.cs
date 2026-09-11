using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    private float maxSpeed = 12.5f;
    private float acceleration = 4f;
    private float currentSpeed;

    private const float FootstepInterval = 0.3f;
    private float footstepTimer;

    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        currentSpeed = player.moveSpeed;
        footstepTimer = 0.1f;
    }

    public override void Update()
    {
        base.Update();

        if (usingStationaryAbility)
            return;

        if (player.movementInput.x == 0 || player.wallDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // Check if player changed direction
        float velocityDirection = Mathf.Sign(rb.linearVelocity.x);
        float inputDirection = Mathf.Sign(player.movementInput.x);

        if(velocityDirection != inputDirection && rb.linearVelocity.x != 0)
        {
            currentSpeed = player.moveSpeed;
        }
        else
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }

        player.SetVelocity(player.movementInput.x * currentSpeed, rb.linearVelocity.y);

        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (Mathf.Abs(rb.linearVelocity.x) <= 0.1f)
            return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            Player.Instance.RaiseFootstep();
            footstepTimer = FootstepInterval;
        }
    }
}