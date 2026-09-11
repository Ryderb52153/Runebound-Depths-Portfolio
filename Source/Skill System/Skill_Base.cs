using System;
using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    protected Skill_Type skillType;
    protected Skill_Upgrade_Type upgradeType;
    protected float cooldown;
    private float lasttimeUsed;

    public Damage_Scale_Data damageScaleData { get; private set; }
    public Player_Skill_Manager skill_Manager { get; private set; }
    public Player player { get; private set; }
    public event Action<Skill_Base> OnSkillUsed;
    public event Action<Skill_Base, float> OnSkillCooldownReduced;
    public float TimeRemainingOnCooldown => Mathf.Max(0, (lasttimeUsed + cooldown) - Time.time);

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        damageScaleData = new Damage_Scale_Data();
    }

    protected virtual void Start()
    {
        skill_Manager = player.skillManager;
    }

    protected bool Oncooldown() => Time.time < lasttimeUsed + cooldown;
    protected bool Unlocked(Skill_Upgrade_Type upgradeToCheck) => upgradeType == upgradeToCheck;
    private void SetSkillOnCooldown() => lasttimeUsed = Time.time;
    public void ResetCooldown() => lasttimeUsed = Time.time - cooldown;
    public Skill_Type GetSkillType() => skillType;
    public Skill_Upgrade_Type GetUpgradeType() => upgradeType;

    public void SetSkillUpgrade(Skill_DataSO skillData)
    {
        UpgradeData upgrade = skillData.upgrade;
        this.upgradeType = upgrade.upgradeType;
        this.cooldown = upgrade.cooldown;
        this.damageScaleData = upgrade.damageScaleData;
        this.skillType = skillData.skillType;
        lasttimeUsed = -cooldown;
    }

    public void ReduceCooldownBy(float cooldownReduction)
    {
        lasttimeUsed -= cooldownReduction;
        OnSkillCooldownReduced?.Invoke(this, lasttimeUsed);
    }

    public virtual void UseSkill()
    {
        SetSkillOnCooldown();
        OnSkillUsed?.Invoke(this);
    }

    public virtual bool CanUseSkill()
    {
        if (upgradeType == Skill_Upgrade_Type.None)
            return false;

        if (Oncooldown())
            return false;

        return true;
    }

    public virtual void OnEndEffect()
    {

    }
}