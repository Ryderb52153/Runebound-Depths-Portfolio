using UnityEngine;

public class Enemy_Vampire_GrabState : EnemyState
{
    private enum GrabPhase { DashStart, Dashing, DashBreak, GrabAttempt, GrabSuccess, Holding, Throw, Complete }
    private GrabPhase currentPhase;

    private Transform player;
    private Vector2 dashTarget;
    private float dashSpeed = 15f;
    private float holdTimer;
    private float holdDuration = 2f; // Total hold time before throw
    private Vector2 throwForce = new Vector2(-50f, 0f);

    public Enemy_Vampire_GrabState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player = Player.Instance.transform;
        currentPhase = GrabPhase.DashStart;
        dashTarget = player.position;
        enemy.HandleFlip(player.position.x > enemy.transform.position.x ? 1 : -1);
        anim.SetTrigger("Vampire_To_Dash");
    }

    public override void Update()
    {
        base.Update();

        switch (currentPhase)
        {
            case GrabPhase.DashStart:
                UpdateDashStartPhase();
                break;
            case GrabPhase.Dashing:
                UpdateDashingPhase();
                break;
            case GrabPhase.DashBreak:
                UpdateDashBreakPhase();
                break;
            case GrabPhase.GrabAttempt:
                UpdateGrabAttemptPhase();
                break;
            case GrabPhase.GrabSuccess:
                UpdateGrabSuccessPhase();
                break;
            case GrabPhase.Holding:
                UpdateHoldingPhase();
                break;
            case GrabPhase.Throw:
                UpdateThrowPhase();
                break;
            case GrabPhase.Complete:
                stateMachine.ChangeState(enemy.BattleState);
                break;
        }
    }

    private void UpdateDashStartPhase()
    {
        if (CheckAnimationFinished("Vampire_To_Dash"))
            currentPhase = GrabPhase.Dashing;
    }

    private void UpdateDashingPhase()
    {
        dashTarget.y = enemy.transform.position.y; // Keep dash horizontal
        Vector2 direction = (dashTarget - (Vector2)enemy.transform.position).normalized;
        enemy.SetVelocity(direction.x * dashSpeed, 0);
        float distanceToTarget = Vector2.Distance(enemy.transform.position, dashTarget);

        if (distanceToTarget < 3.5f)
            StartDashBreak();
    }

    private void UpdateDashBreakPhase()
    {
        enemy.SetVelocity(0, 0);

        if (CheckAnimationFinished("Vampire_Dash_Break"))
            StartGrabAttempt();
    }

    private void UpdateGrabAttemptPhase()
    {
        enemy.SetVelocity(0, 0);

        if (CheckAnimationFinished("Vampire_To_Grab"))
        {
            if (IsPlayerInGrabRange())
            {
                SuccessfulGrab();
            }
            else
            {
                // Missed the grab
                currentPhase = GrabPhase.Complete;
            }
        }
    }

    private void UpdateGrabSuccessPhase()
    {
        enemy.SetVelocity(0, 0);
        AttachPlayerToVampire();

        if (CheckAnimationFinished("Vampire_Grab"))
            StartHolding();
    }

    private void UpdateHoldingPhase()
    {
        enemy.SetVelocity(0, 0);
        holdTimer += Time.deltaTime;

        if (holdTimer >= holdDuration)
            StartThrow();
    }

    private void UpdateThrowPhase()
    {
        if (CheckAnimationFinished("Vampire_Grab_Out"))
            currentPhase = GrabPhase.Complete;
    }

    private void StartDashBreak()
    {
        currentPhase = GrabPhase.DashBreak;
        anim.SetTrigger("Vampire_Dash_Break");
    }

    private void StartGrabAttempt()
    {
        currentPhase = GrabPhase.GrabAttempt;
        anim.SetTrigger("Vampire_To_Grab");
    }

    private void SuccessfulGrab()
    {
        currentPhase = GrabPhase.GrabSuccess;
        anim.SetTrigger("Vampire_Grab");

        Player.Instance.SwitchToGrabState();
        Player.Instance.HandleFlip(enemy.facingDirection);
        Player.Instance.inputSet.Disable();
    }

    private void StartHolding()
    {
        currentPhase = GrabPhase.Holding;
        holdTimer = 0f;
        anim.SetBool("Vampire_Grabbing", true);
    }

    private void StartThrow()
    {
        currentPhase = GrabPhase.Throw;
        anim.SetBool("Vampire_Grabbing", false);
        ThrowPlayer();
    }

    private void ThrowPlayer()
    {
        Player.Instance.rb.linearVelocity = new Vector2(throwForce.x * enemy.facingDirection, throwForce.y);
        Player.Instance.inputSet.Enable();
        Player.Instance.SwitchToThrowState();
    }

    private void AttachPlayerToVampire()
    {
        // Position player in front of vampire (adjust offset as needed)
        Vector2 holdOffset = new Vector2(enemy.facingDirection * 2f, -.75f);
        Player.Instance.gameObject.transform.position = (Vector2)enemy.transform.position + holdOffset;
    }

    private bool IsPlayerInGrabRange()
    {
        float grabRange = 4f;
        float distance = Vector2.Distance(enemy.transform.position, player.position);
        return distance <= grabRange;
    }

    private bool CheckAnimationFinished(string animName)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animName) 
            && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f;
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("Vampire_Grabbing", false);

        if (currentPhase == GrabPhase.Holding || currentPhase == GrabPhase.GrabSuccess)
        {
            Player.Instance.inputSet.Enable();
            Player.Instance.SwitchToThrowState();
        }
    }
}