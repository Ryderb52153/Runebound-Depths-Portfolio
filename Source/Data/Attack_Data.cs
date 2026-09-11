using System;
using UnityEngine;

[Serializable]
public class Attack_Data
{
    public float physicalDamage;
    public float elementalDamage;
    public float armorPenetration;
    public Vector3 attackerPosition;
    public Transform Transform { get; set; }
    public DamageType damageType;
    public bool isCrit;
    public bool vfxOverride = false;
    public ElementType element;
    public Elemental_Effect_Data elementalEffectData;

    public Attack_Data(Entity_Stats entityStats, AttackDefinition definition, Vector3 attackerPos)
    {
        damageType = definition.damageType;
        attackerPosition = attackerPos;
        Transform = entityStats.transform;
        armorPenetration = entityStats.GetArmorReduction();
        vfxOverride = definition.vfxOverride;

        Damage_Scale_Data scaleData = definition.damageScale;

        // 1. Physical Damage
        if (damageType == DamageType.Physical || damageType == DamageType.Both)
        {
            physicalDamage = entityStats.GetPhysicalDamage(out isCrit, scaleData.physical);
        }

        // 2. Elemental Damage
        if (damageType == DamageType.Elemental || damageType == DamageType.Both)
        {
            if (definition.elementOverride != ElementType.None)
            {
                element = definition.elementOverride;
                elementalDamage = entityStats.GetElementalDamageFor(element, scaleData.elemental);
            }
            else
            {
                elementalDamage = entityStats.GetHighestElementalDamage(out element, scaleData.elemental);
            }

            elementalEffectData = new Elemental_Effect_Data(entityStats, scaleData);
        }

        // Apply Player Passive Skill Buff Multiplier if applicable
        if (entityStats is Player_Stats playerStats)
        {
            var skillManager = playerStats.GetComponent<Player_Skill_Manager>();
            if (skillManager != null)
            {
                float passiveMultiplier = skillManager.GetPassiveSkillMultiplier();
                physicalDamage *= passiveMultiplier;
                elementalDamage *= passiveMultiplier;
            }
        }
    }

    public Attack_Data(Entity_Stats entityStats, Damage_Scale_Data scaleData)
    {
        damageType = DamageType.Both;
        attackerPosition = entityStats.transform.position;
        Transform = entityStats.transform;
        armorPenetration = entityStats.GetArmorReduction();

        physicalDamage = entityStats.GetPhysicalDamage(out isCrit, scaleData.physical);
        elementalDamage = entityStats.GetHighestElementalDamage(out element, scaleData.elemental);

        elementalEffectData = new Elemental_Effect_Data(entityStats, scaleData);

        // Apply Player Passive Skill Buff Multiplier if applicable
        if (entityStats is Player_Stats playerStats)
        {
            var skillManager = playerStats.GetComponent<Player_Skill_Manager>();
            if (skillManager != null)
            {
                float passiveMultiplier = skillManager.GetPassiveSkillMultiplier();
                physicalDamage *= passiveMultiplier;
                elementalDamage *= passiveMultiplier;
            }
        }
    }
}
