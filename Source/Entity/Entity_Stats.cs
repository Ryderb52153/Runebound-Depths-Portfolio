using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    // Default SO that can be used to set base values in the inspector and reset to those values when needed
    public Stat_SetupSO defaultStatSetup;

    // public stats 
    public Stat_OffenseGroup offenseStats;
    public Stat_DefenseGroup defenseStats;
    public Stat_ResourceGroup resourceStats;
    public Stat_MajorGroup majorStats;

    protected virtual void Awake()
    {
        
    }

    public Attack_Data GetAttackData(AttackDefinition attackDefinition)
    {
        return new Attack_Data(this, attackDefinition, transform.position);
    }

    // New overload to force a specific element
    public float GetElementalDamageFor(ElementType forcedElement, float scaleFactor = 1)
    {
        if (forcedElement == ElementType.None) return 0;

        float fireDamage = offenseStats.fireDamage.GetValue();
        float iceDamage = offenseStats.iceDamage.GetValue();
        float lightningDamage = offenseStats.lightningDamage.GetValue();
        float bonusElementalDamage = majorStats.intelligence.GetValue();

        float baseDamage = forcedElement switch
        {
            ElementType.Fire => fireDamage,
            ElementType.Ice => iceDamage,
            ElementType.Lightning => lightningDamage,
            _ => 0
        };

        // Keep your logic of 50% bonus from other elements
        float bonusFire = (forcedElement == ElementType.Fire) ? 0 : fireDamage * .5f;
        float bonusIce = (forcedElement == ElementType.Ice) ? 0 : iceDamage * .5f;
        float bonusLightning = (forcedElement == ElementType.Lightning) ? 0 : lightningDamage * .5f;
        float weakerElementalDamage = bonusFire + bonusIce + bonusLightning;

        return (baseDamage + bonusElementalDamage + weakerElementalDamage) * scaleFactor;
    }

    public float GetHighestElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offenseStats.fireDamage.GetValue();
        float iceDamage = offenseStats.iceDamage.GetValue();
        float lightningDamage = offenseStats.lightningDamage.GetValue();
        float bonusElementalDamage = majorStats.intelligence.GetValue();

        float highestDamge = fireDamage;
        element = ElementType.Fire;

        if (iceDamage > highestDamge)
        {
            highestDamge = iceDamage;
            element = ElementType.Ice;
        }

        if (lightningDamage > highestDamge)
        {
            highestDamge = lightningDamage;
            element = ElementType.Lightning;
        }

        if(highestDamge <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * .5f;
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * .5f;
        float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * .5f;
        float weakerElementalDamage = bonusFire + bonusIce + bonusLightning;

        highestDamge += bonusElementalDamage + weakerElementalDamage;
        return highestDamge * scaleFactor;
    }

    public float GetElementalResistance(ElementType element)
{
        float baseResistance = 0;
        float bonuseResistance = majorStats.intelligence.GetValue() * 0.5f;

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defenseStats.fireRes.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defenseStats.iceRes.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defenseStats.lightningRes.GetValue();
                break;
            default:
                return 0;
        }

        float totalResistance = baseResistance + bonuseResistance;
        float resistanceCap = 75f;

        if (totalResistance > resistanceCap)
            return resistanceCap / 100;

        return totalResistance / 100;
    }

    public float GetPhysicalDamage(out bool isCriticalHit, float scaleFactor = 1)
    {
        float basePhysDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critPower = GetCritPower() / 100f;

        isCriticalHit = Random.Range(0, 100) < critChance;
        float finalDamage = isCriticalHit ? basePhysDamage * critPower : basePhysDamage;

        return finalDamage * scaleFactor;
    }

    private float GetBaseDamage() => offenseStats.damage.GetValue() + majorStats.strength.GetValue();
    private float GetCritChance() => offenseStats.critChance.GetValue() + (majorStats.agility.GetValue() * 0.3f);
    private float GetCritPower() => offenseStats.critPower.GetValue() + (majorStats.strength.GetValue() * 0.5f);
    private float GetBaseArmor() => defenseStats.armor.GetValue() + majorStats.vitality.GetValue();
    private float GetIceDamage() => Mathf.Round((offenseStats.iceDamage.GetValue() + majorStats.intelligence.GetValue()) * offenseStats.iceDamage.Multiplier);   
    private float GetFireDamage() => Mathf.Round((offenseStats.fireDamage.GetValue() + majorStats.intelligence.GetValue()) * offenseStats.fireDamage.Multiplier);
    private float GetLightningDamage() => Mathf.Round((offenseStats.lightningDamage.GetValue() + majorStats.intelligence.GetValue()) * offenseStats.lightningDamage.Multiplier);

    public float GetArmorMitigation(float armorReduction)
    {
        float armorValue = GetBaseArmor();

        float reducedArmorMultiplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveAmror = armorValue * reducedArmorMultiplier;

        float mitigation = effectiveAmror / (effectiveAmror + 100);
        float mitigationCap = 0.85f;

        if (mitigation > mitigationCap)
            return mitigationCap;

        return mitigation;
    }

    public float GetArmorReduction()
    {
        return offenseStats.armorReduction.GetValue() / 100;
    }

    public float GetEvasion()
    {
        float baseEvasion = defenseStats.evasion.GetValue();
        float agilityBonus = majorStats.agility.GetValue() * 0.5f;
        float evasionCap = 85f;

        if ((baseEvasion + agilityBonus) > evasionCap)
            return evasionCap;

        return baseEvasion + agilityBonus;
    }

    public float GetMaxHealth()
    {
        float baseHp = resourceStats.maxHealth.GetValue();
        float bonusHp = majorStats.vitality.GetValue() * 5;

        return baseHp + bonusHp;
    }

    public Stat GetStatByType(Stat_Type statType)
    {
        return statType switch
        {
            Stat_Type.Strength => majorStats.strength,
            Stat_Type.Agility => majorStats.agility,
            Stat_Type.Vitality => majorStats.vitality,
            Stat_Type.Intelligence => majorStats.intelligence,
            Stat_Type.Damage => offenseStats.damage,
            Stat_Type.CritChance => offenseStats.critChance,
            Stat_Type.CritPower => offenseStats.critPower,
            Stat_Type.FireDamage => offenseStats.fireDamage,
            Stat_Type.IceDamage => offenseStats.iceDamage,
            Stat_Type.LightningDamage => offenseStats.lightningDamage,
            Stat_Type.ArmorReduction => offenseStats.armorReduction,
            Stat_Type.Armor => defenseStats.armor,
            Stat_Type.Evasion => defenseStats.evasion,
            Stat_Type.AttackSpeed => offenseStats.attackSpeed,
            Stat_Type.FireResistance => defenseStats.fireRes,
            Stat_Type.IceResistance => defenseStats.iceRes,
            Stat_Type.LightningResistance => defenseStats.lightningRes,
            Stat_Type.MaxHealth => resourceStats.maxHealth,
            Stat_Type.HealthRegen => resourceStats.healthRegen,
            _ => LogAndReturnNull(statType),
        };
    }

    public float GetAttributeValue(Stat_Type statType)
    {
        return statType switch
        {
            Stat_Type.Strength => majorStats.strength.GetValue(),
            Stat_Type.Agility => majorStats.agility.GetValue(),
            Stat_Type.Vitality => majorStats.vitality.GetValue(),
            Stat_Type.Intelligence => majorStats.intelligence.GetValue(),
            Stat_Type.Damage => GetBaseDamage(),
            Stat_Type.CritChance => GetCritChance(),
            Stat_Type.CritPower => GetCritPower(),
            Stat_Type.ArmorReduction => GetArmorReduction() * 100,
            Stat_Type.AttackSpeed => offenseStats.attackSpeed.GetValue() * 100,
            Stat_Type.MaxHealth => GetMaxHealth(),
            Stat_Type.HealthRegen => resourceStats.healthRegen.GetValue(),
            Stat_Type.Evasion => GetEvasion(),
            Stat_Type.Armor => GetBaseArmor(),
            Stat_Type.IceDamage => GetIceDamage(),
            Stat_Type.FireDamage => GetFireDamage(),
            Stat_Type.LightningDamage => GetLightningDamage(),
            Stat_Type.ElementalDamage => GetHighestElementalDamage(out ElementType element, 1),
            Stat_Type.IceResistance => GetElementalResistance(ElementType.Ice) * 100,
Stat_Type.FireResistance => GetElementalResistance(ElementType.Fire) * 100,
            Stat_Type.LightningResistance => GetElementalResistance(ElementType.Lightning) * 100,
            _ => 0f,
        };
    }

    private Stat LogAndReturnNull(Stat_Type statType)
    {
        Debug.Log($"Stat type not found: {statType}");
        return null;
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if(defaultStatSetup == null)
        {
            Debug.LogWarning("Default Stat Setup is not assigned.");
            return;
        }

        resourceStats.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resourceStats.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);
        offenseStats.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offenseStats.damage.SetBaseValue(defaultStatSetup.damage);
        offenseStats.critChance.SetBaseValue(defaultStatSetup.critChance);
        offenseStats.critPower.SetBaseValue(defaultStatSetup.critPower);
        offenseStats.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offenseStats.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offenseStats.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);
        defenseStats.armor.SetBaseValue(defaultStatSetup.armor);    
        defenseStats.evasion.SetBaseValue(defaultStatSetup.evasion);
        defenseStats.fireRes.SetBaseValue(defaultStatSetup.fireResistance);
        defenseStats.iceRes.SetBaseValue(defaultStatSetup.iceResistance);
        defenseStats.lightningRes.SetBaseValue(defaultStatSetup.lightningResistance);
        majorStats.strength.SetBaseValue(defaultStatSetup.strength);
        majorStats.agility.SetBaseValue(defaultStatSetup.agility);
        majorStats.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        majorStats.vitality.SetBaseValue(defaultStatSetup.vitality);
    }

    public float GetStatUpgradeAmount(Stat_Type statType)
    {
        return statType switch
        {
            Stat_Type.MaxHealth => 10f,
            Stat_Type.HealthRegen => 0.1f,
            Stat_Type.AttackSpeed => 0.01f,
            Stat_Type.Damage => 1f,
            Stat_Type.CritChance => 1f,
            Stat_Type.CritPower => 1f,
            Stat_Type.Armor => 1f,
            Stat_Type.Evasion => 1f,
            Stat_Type.Strength => 1f,
            Stat_Type.Agility => 1f,
            Stat_Type.Intelligence => 1f,
            Stat_Type.Vitality => 1f,
            Stat_Type.FireDamage => 1f,
            Stat_Type.IceDamage => 1f,
            Stat_Type.LightningDamage => 1f,
            Stat_Type.FireResistance => 1f,
            Stat_Type.IceResistance => 1f,
            Stat_Type.LightningResistance => 1f,
            Stat_Type.ArmorReduction => 1f,
            _ => 1f,
        };
    }

    public void AddUnlockedStatToBase(Stat_Type stat, float value)
    {
        Stat statToChange = GetStatByType(stat);

        if (statToChange == null) 
        {
            Debug.LogWarning("Stat not found: " + stat);
            return;
        }

        float amountToAdd = value * GetStatUpgradeAmount(stat);
        float calculatedValue = statToChange.GetBaseValue() + amountToAdd;
        statToChange.SetBaseValue(calculatedValue);
    }

    public void RemoveUnlockedStatFromBase(Stat_Type stat, float value)
    {
        Stat statToChange = GetStatByType(stat);

        if (statToChange == null) 
        {
            Debug.LogWarning("Stat not found: " + stat);
            return;
        }

        float amountToRemove = value * GetStatUpgradeAmount(stat);
        float calculatedValue = statToChange.GetBaseValue() - amountToRemove;
        statToChange.SetBaseValue(calculatedValue);
        Debug.Log($"Removed {amountToRemove} from {stat}. New base value: {statToChange.GetBaseValue()}");
    }

    public string GetStatDescription(Stat_Type statType)
    {
        switch (statType)
        {
            //Major Stats
            case Stat_Type.Strength:
                return "Increases physical damage by 1 per point. " +
                       "\n Increases critical power by 0.5% per point.";
            case Stat_Type.Agility:
                return "Increases evasion by 0.5% per point. " +
                       "\n Increases critical chance by 0.3% per point.";
            case Stat_Type.Vitality:
                return "Increases maximum health by 5 per point. " +
                       "\n Increases Armor by 1 per point.";
            case Stat_Type.Intelligence:
                return "Increases elemental damage by 1 per point. " +
                       "\n Increases elemental resistances by 0.5% per point."+
                       "\n If all elements have 0 damage, no elemental damage is dealt.";
            //Physical Damage Stats
            case Stat_Type.Damage:
                return "Increases base physical damage.";
            case Stat_Type.CritChance:
                return "Increases chance to deal critical hits.";
            case Stat_Type.CritPower:
                return "Increases the damage multiplier of critical hits.";
            case Stat_Type.ArmorReduction:
                return "Percent of armor that will be ignored by your attacks";
            case Stat_Type.AttackSpeed:
                return "Determines how quickly you can attack.";
            //Deffenseive Stats
            case Stat_Type.MaxHealth:
                return "Determines the maximum amount of health you can have.";
            case Stat_Type.HealthRegen:
                return "Determines how much health you regenerate per second.";
            case Stat_Type.Evasion:
                return "Chance to completely avoid an incoming physical attack." +
                       "\n Evasion is capped at 85%.";
            case Stat_Type.Armor:
                return "Reduces incoming physical damage." +
                       "\n Damage Mitigation = Armor / (Armor + 100) " +
                       "\n Maximum Damage Mitigation is capped at 85%.";
            //Elemental Damage
            case Stat_Type.FireDamage:
                return "Determines the fire damage of your attacks.";
            case Stat_Type.IceDamage:
                return "Determines the ice damage of your attacks.";
            case Stat_Type.LightningDamage:
                return "Determines the lightning damage of your attacks.";
            case Stat_Type.ElementalDamage:
                return "Elemental damage combines all three elements. " +
                       "\n The highest element is the main element, applies corresponding element status" +
                       "\n and the others provide 50% of thier bonus damage.";
            //Elemental Resistance
            case Stat_Type.FireResistance:
                return "Reduces fire damage taken.";
            case Stat_Type.IceResistance:
                return "Reduces Ice damage taken.";
            case Stat_Type.LightningResistance:
                return "Reduces Lightning damage taken.";

            default:
                return "No Tooltip available for this stat.";
        }
    }
}

public enum ElementType
{
    None,
    Fire,
    Ice,
    Lightning
}