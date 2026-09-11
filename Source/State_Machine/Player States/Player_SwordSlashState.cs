using UnityEngine;

public class Player_SwordSlashState : PlayerState
{
    private enum SlashPhase { Begin, Charging, Attack }
    private SlashPhase currentPhase;
    
    private float chargeTime;
    private float maxChargeTime = 1.25f;
    private AudioSource chargeSFX;

    public Player_SwordSlashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0f, 0f);

        currentPhase = SlashPhase.Begin;
        chargeTime = 0f;

        chargeSFX =SoundManager.Instance.PlaySFX(SoundName.Ability_Sword_Slash_Start, player.transform.position);

        anim.SetBool("swordSlashBegin", true);
        anim.SetBool("swordSlashCharge", false);
        anim.SetBool("swordSlashAttack", false);
    }

    public override void Update()
    {
        switch (currentPhase)
        {
            case SlashPhase.Begin:
                UpdateBeginPhase();
                break;
            case SlashPhase.Charging:
                UpdateChargingPhase();
                break;
            case SlashPhase.Attack:
                UpdateAttackPhase();
                break;
        }

        if(triggerCalled)
        {
            triggerCalled = false;
            skillManager.UseSkill(3);
        }
    }

    private void UpdateBeginPhase()
    {
        if (!input.Player.Third_Ability.IsPressed())
            stateMachine.ChangeState(player.idleState);

        float normalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        
        if (normalizedTime >= 0.95f)
            StartCharging();
    }

    private void UpdateChargingPhase()
    {
        chargeTime += Time.deltaTime;

        // Check if button was released and max charge reached
        if (!input.Player.Third_Ability.IsPressed())
            StartAttack();
    }

    private void UpdateAttackPhase()
    {
        float normalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        
        // Check if attack animation has finished
        if (normalizedTime >= 0.95f)
            stateMachine.ChangeState(player.idleState);
    }

    private void StartCharging()
    {
        currentPhase = SlashPhase.Charging;
        
        anim.SetBool("swordSlashBegin", false);
        anim.SetBool("swordSlashCharge", true);
    }

    private void StartAttack()
    {
        currentPhase = SlashPhase.Attack;

        anim.SetBool("swordSlashBegin", false);
        anim.SetBool("swordSlashCharge", false);
        anim.SetBool("swordSlashAttack", true);

        float clampedChargeTime = Mathf.Min(chargeTime, maxChargeTime);
        float damageMultiplier = 1f + (clampedChargeTime / maxChargeTime);
        skillManager.swordSlash.SetDamageMultiplier(damageMultiplier);

        HandleSFX();
        SoundManager.Instance.PlaySFX(SoundName.Ability_Sword_Slash_End, player.transform.position);
    }

    private void HandleSFX()
    {
        // Stop the charge sound immediately when attacking
        if (chargeSFX != null)
        {
            chargeSFX.Stop();
            chargeSFX = null;
        }
    }

    public override void Exit()
    {
        base.Exit();
        HandleSFX();
        usingStationaryAbility = false;

        anim.SetBool("swordSlashBegin", false);
        anim.SetBool("swordSlashCharge", false);
        anim.SetBool("swordSlashAttack", false);
    }
}