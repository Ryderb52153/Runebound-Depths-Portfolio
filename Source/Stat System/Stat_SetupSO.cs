using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("Resources")]
    public float maxHealth = 0;
    public float healthRegen = 0;

    [Header("Offense - Physical Damage")]
    public float attackSpeed = 0;
    public float damage = 0;
    public float critChance;
    public float critPower = 0;

    [Header("Offsense - Elemental Damage")]
    public float fireDamage;
    public float iceDamage;
    public float lightningDamage;

    [Header("Defense - Physical Damage")]
    public float armor;
    public float evasion;

    [Header("Defense - Elemental Damage")]
    public float fireResistance;
    public float iceResistance;
    public float lightningResistance;

    [Header("Major Stats")]
    public float strength;
    public float agility;
    public float intelligence;
    public float vitality;

    public void UpdateState(Stat_Type type, float value)
    {
        switch (type)
        {
            case Stat_Type.MaxHealth:
                maxHealth += value;
                break;
            case Stat_Type.HealthRegen:
                healthRegen += value;
                break;
            case Stat_Type.Strength:
                strength += value;
                break;
            case Stat_Type.Agility:
                agility += value;
                break;
            case Stat_Type.Intelligence:
                intelligence += value;
                break;
            case Stat_Type.Vitality:
                vitality += value;
                break;
            case Stat_Type.AttackSpeed:
                attackSpeed += value;
                break;
            case Stat_Type.Damage:
                damage += value;
                break;
            case Stat_Type.CritChance:
                critChance += value;
                break;
            case Stat_Type.CritPower:
                critPower += value;
                break;
            case Stat_Type.FireDamage:
                fireDamage += value;
                break;
            case Stat_Type.IceDamage:
                iceDamage += value;
                break;
            case Stat_Type.LightningDamage:
                lightningDamage += value;
                break;
            case Stat_Type.Armor:
                armor += value;
                break;
            case Stat_Type.Evasion:
                evasion += value;
                break;
            case Stat_Type.IceResistance:
                iceResistance += value;
                break;
            case Stat_Type.FireResistance:
                fireResistance += value;
                break;
            case Stat_Type.LightningResistance:
                lightningResistance += value;
                break;
        }

    }

    public float GetStatValue(Stat_Type type)
    {
        return type switch
        {
            Stat_Type.MaxHealth => maxHealth,
            Stat_Type.HealthRegen => healthRegen,
            Stat_Type.Strength => strength,
            Stat_Type.Agility => agility,
            Stat_Type.Intelligence => intelligence,
            Stat_Type.Vitality => vitality,
            Stat_Type.AttackSpeed => attackSpeed,
            Stat_Type.Damage => damage,
            Stat_Type.CritChance => critChance,
            Stat_Type.CritPower => critPower,
            Stat_Type.FireDamage => fireDamage,
            Stat_Type.IceDamage => iceDamage,
            Stat_Type.LightningDamage => lightningDamage,
            Stat_Type.Armor => armor,
            Stat_Type.Evasion => evasion,
            Stat_Type.IceResistance => iceResistance,
            Stat_Type.FireResistance => fireResistance,
            Stat_Type.LightningResistance => lightningResistance,
            _ => 0
        };
    }
}