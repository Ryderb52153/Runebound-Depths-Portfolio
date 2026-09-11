using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private Skill_Object_TimeEcho timeEchoPrefab;
    [SerializeField] private float echoDuration;

    [Header("Attack Upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float duplicateChance = .3f;

    [Header("Heal Wisp Upgrade")]
    [SerializeField] private float damagePercentHealed = .3f;
    [SerializeField] private float cooldownReducedInSeconds;

    public float GetEchoDuration => echoDuration;

    protected override void Awake()
    {
        base.Awake();
        skillType = Skill_Type.TimeEcho;
    }

    public override void UseSkill()
    {
        base.UseSkill();
        CreateTimeEcho();
    }

    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? player.transform.position;

        Skill_Object_TimeEcho timeEcho = ObjectPoolManager.SpawnObject(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.SetupEcho(this);
    }

    public float GetPercentOfDamageHealed()
    {
        if(ShouldBeWisp())
            return damagePercentHealed;
        return 0f;
    }

    public float GetcooldownReducedInSeconds()
    {
        if(upgradeType == Skill_Upgrade_Type.TimeEcho_CooldownWisp)
            return cooldownReducedInSeconds;
        return 0f;
    }

    public bool CanRemoveNegativeEffects()
    {
        return upgradeType == Skill_Upgrade_Type.TimeEcho_CleanseWisp;
    }

    public float GetDuplicateChance()
    {
        if(upgradeType == Skill_Upgrade_Type.TimeEcho_ChanceToMultiply)
            return duplicateChance;

        return 0f;  
    }

    public bool ShouldBeWisp()
    {
        return upgradeType == Skill_Upgrade_Type.TimeEcho_HealWisp
            || upgradeType == Skill_Upgrade_Type.TimeEcho_CleanseWisp
            || upgradeType == Skill_Upgrade_Type.TimeEcho_CooldownWisp;
    }


    public int GetMaxAttacks()
    {
        if (upgradeType == Skill_Upgrade_Type.TimeEcho_SingleAttack || upgradeType == Skill_Upgrade_Type.TimeEcho_ChanceToMultiply)
            return 1;
               
        if(upgradeType == Skill_Upgrade_Type.TimeEcho_MultiAttack)
            return maxAttacks;

        return 0;
    }
}