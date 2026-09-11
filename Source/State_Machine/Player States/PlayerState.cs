
public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_Skill_Manager skillManager;
    protected static bool usingStationaryAbility;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName): base(stateMachine, animBoolName)
    {
        this.player = player;
        this.anim = player.anim;
        this.rb = player.rb;
        this.input = player.inputSet;
        this.stats = player.stats;
        this.skillManager = player.skillManager;
    }

    public override void Update()
    {
        base.Update();

        if(usingStationaryAbility) 
            return;

        if (input.Player.First_Ability.WasPressedThisFrame())
            UseFirstAbility();
        else if(input.Player.Second_Ability.WasPressedThisFrame())
            UseSecondAbility();
        else if(input.Player.Third_Ability.WasPressedThisFrame())
            UseThirdAbility();
        else if (input.Player.Ultimate_Ability.WasPressedThisFrame())
            UseUltimateAbility();
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    protected void UseFirstAbility()
    {
        if(skillManager.CanUseSkill(1) && CanDash())
        {
            skillManager.UseSkill(1);
            stateMachine.ChangeState(player.dashState);
        }
    }

    private void UseSecondAbility()
    {
        if (skillManager.CanUseSkill(2))
        {
            skillManager.UseSkill(2);
        }
    }

    private void UseThirdAbility()
    {
        if (skillManager.CanUseSkill(3) && player.groundDetected)
        {
            usingStationaryAbility = true;
            stateMachine.ChangeState(player.swordSlashState);
        }
    }

    private void UseUltimateAbility()
    {
        if(skillManager.CanUseSkill(4))
        {
            skillManager.UseSkill(4);

            if (!skillManager.domainExpansion.JustDomain())
                stateMachine.ChangeState(player.domainExpansionState);
        }
    }

    protected bool CanDash()
    {
        if(player.wallDetected)
            return false;

        if(stateMachine.currentState == player.dashState || stateMachine.currentState == player.domainExpansionState)
            return false;

        return true;
    }
}