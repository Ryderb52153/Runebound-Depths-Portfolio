using UnityEngine;

public class Enemy_Goblin_AttackState : Enemy_AttackState
{
    private AttackConfig currentAttack;

    // Dash Attack Specific
    private float attackTimer;
    private bool isDashing;
    private float dashSpeed = 12;
    private float playerPreviousLocationX;

    // Tornado Attack Specific
    private bool isTornadoSpinning;
    private float tornadoMoveSpeed = 16f;
    private float tornadoDuration = 5f;
    private float tornadoBounceCooldown = 0.1f;
    private float tornadoBounceTimer;
    private int tornadoDirection;
    private bool tornadoEnding;

    public Enemy_Goblin_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        currentAttack = enemy.currentAttack;
        anim.SetBool(currentAttack.animationTrigger, true);
        triggerCalled = false;
        attackTimer = 0f;
        SyncAttackSpeed();

        if(currentAttack.attackName == "Dash Attack")
        {
            SetUpDashAttack();
        }
        else if (currentAttack.attackName == "Tornado Attack")
        {
            SetupTornadoAttack();
        }
    }


    public override void Update()
    {
        base.Update();
        attackTimer += Time.deltaTime;
        HandleDashAttackMovement();
        HandleTornadoAttackMovement();
    }

    public override void Exit()
    {
        anim.SetBool(currentAttack.animationTrigger, false);
        anim.SetBool("tornadoAttackEnd", false);
        enemy.SetVelocity(0f, rb.linearVelocity.y);
    }
    
    private void HandleDashAttackMovement()
    {
        if (currentAttack == null || currentAttack.attackName != "Dash Attack")
            return;

        float dashStart = .08f;
        float dashEnd = dashStart + 1f;
        float stopDistance = 5f;
        AnimationCurve curve = AnimationCurve.Linear(0,1,1,1);

        if (attackTimer >= dashStart && attackTimer <= dashEnd)
        {
            Vector2 currentPosition = enemy.transform.position;
            float distanceToTarget = Mathf.Abs(playerPreviousLocationX - currentPosition.x);

            if (distanceToTarget <= stopDistance)
            {
                isDashing = false;
                enemy.SetVelocity(0f, rb.linearVelocity.y);
                return;
            }

            isDashing = true;

            float dashProgress = Mathf.InverseLerp(dashStart, dashEnd, attackTimer);
            float speedMultiplier = curve.Evaluate(dashProgress);

            float moveDirection = Mathf.Sign(playerPreviousLocationX - currentPosition.x);

            enemy.SetVelocity(moveDirection * dashSpeed * speedMultiplier, rb.linearVelocity.y);
        }
        else if (isDashing)
        {
            isDashing = false;
            enemy.SetVelocity(0f, rb.linearVelocity.y);
        }
    }

    private void SetUpDashAttack()
    {
        playerPreviousLocationX = enemy.GetPlayer().position.x;
        isDashing = false;
    }

    private void SetupTornadoAttack()
    {
        isTornadoSpinning = true;
        tornadoEnding = false;
        tornadoBounceTimer = 0f;

        Transform player = enemy.GetPlayer();

        if (player != null)
            tornadoDirection = player.position.x >= enemy.transform.position.x ? 1 : -1;
        else
            tornadoDirection = enemy.facingDirection;

        enemy.HandleFlip(tornadoDirection);
    }

    private void HandleTornadoAttackMovement()
    {
        if (currentAttack == null || currentAttack.attackName != "Tornado Attack")
            return;

        if (!isTornadoSpinning || tornadoEnding)
            return;

        if (tornadoBounceTimer > 0f)
            tornadoBounceTimer -= Time.deltaTime;

        enemy.SetVelocity(tornadoDirection * tornadoMoveSpeed, rb.linearVelocity.y);

        if (enemy.wallDetected && tornadoBounceTimer <= 0f)
        {
            tornadoDirection *= -1;
            tornadoBounceTimer = tornadoBounceCooldown;
            enemy.HandleFlip(tornadoDirection);
        }

        if (attackTimer >= tornadoDuration)
        {
            EndTornadoAttack();
        }
    }

    private void EndTornadoAttack()
    {
        tornadoEnding = true;
        isTornadoSpinning = false;

        enemy.SetVelocity(0f, rb.linearVelocity.y);
        anim.SetBool("tornadoAttackEnd", true);
    }
}
