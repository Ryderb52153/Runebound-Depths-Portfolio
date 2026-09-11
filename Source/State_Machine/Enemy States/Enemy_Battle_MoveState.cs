using UnityEngine;

public class Enemy_Battle_MoveState : Enemy_GroundedState
{
    public Enemy_Battle_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public Transform player;
    public float desiredRange;

    private const float FootstepInterval = 0.4f;
    private float footstepTimer;

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
        footstepTimer = 0.1f;
    }

    public override void Update()
    {
        UpdateAnimationParameters();

        // Keep chasing the highest priority target (echo first, then player);
        // re-targets the player automatically when a chased echo disappears.
        Transform currentTarget = enemy.GetCurrentTarget();
        if (currentTarget != null)
            player = currentTarget;

        if (Mathf.Abs(player.position.y - enemy.transform.position.y) > 3.5f)
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        if (DistanceToPlayer() <= desiredRange)
        {
            stateMachine.ChangeState(enemy.BattleState);
        }
        else
        {
            enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocity.y);
            HandleFootsteps();
        }
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

    protected int DirectionToPlayer()
    {
        if (player == null)
            return 0;

        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }

    protected float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Vector2.Distance(player.position, enemy.transform.position);
    }
}
