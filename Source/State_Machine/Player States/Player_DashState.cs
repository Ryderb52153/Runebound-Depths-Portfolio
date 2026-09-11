using UnityEngine;

public class Player_DashState : PlayerState
{
    private float originalGravityScale;
    private int dashDirection;

    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.vfx.DoImageEchoEffect(player.dashDuration);
        stateTimer = player.dashDuration;
        originalGravityScale = rb.gravityScale;
        dashDirection = player.movementInput.x != 0 ? (int)player.movementInput.x : player.facingDirection;
        rb.gravityScale = 0f;

        SoundManager.Instance.PlaySFX(SoundName.Player_Dash, player.transform.position);
        player.health.SetCanTakeDamage(false);
    }

    override public void Update()
    {
        base.Update();
        CheckDetection();
        player.SetVelocity(player.dashSpeed * dashDirection, 0f);

        if(stateTimer <= 0)
        {
            if(player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    override public void Exit()
    {
        base.Exit();
        skillManager.GetSkillByType(Skill_Type.Dash).OnEndEffect();
        player.SetVelocity(0f, 0f);
        rb.gravityScale = originalGravityScale;
        player.health.SetCanTakeDamage(true);
    }

    private void CheckDetection()
    {
        if (player.wallDetected)
        {
            if(player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }
}