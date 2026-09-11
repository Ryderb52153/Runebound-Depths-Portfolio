using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    private const float FootstepInterval = 0.45f;
    private float footstepTimer;

    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {

    }

    override public void Enter()
    {
        base.Enter();

        footstepTimer = 0.1f;

        if(enemy.groundDetected == false || enemy.wallDetected)
            enemy.Flip();
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDirection, rb.linearVelocity.y);
        
        if(enemy.groundDetected == false || enemy.wallDetected)
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (Mathf.Abs(rb.linearVelocity.x) <= 0.1f)
            return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            Enemy.RaiseFootstep(enemy.transform.position);
            footstepTimer = FootstepInterval;
        }
    }
}