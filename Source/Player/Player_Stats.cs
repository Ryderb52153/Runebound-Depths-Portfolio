using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Stats : Entity_Stats, IProfileSaveable
{
    [Serializable]
    public class ActiveBuff
    {
        public string source;
        public float duration;
        public float endTime;
        public Sprite icon;
        public Buff_Effect_Data[] buffs;

        public float RemainingTime => Mathf.Max(0f, endTime - Time.time);
    }

    private Stat_SetupSO storedPlayerUnlockedStats;

    public List<ActiveBuff> activeBuffsList = new List<ActiveBuff>();
    public int statPoints;
    public event Action OnStatPointsChanged;
    public event Action OnUnlockedStatsChanged;
    public event Action OnBuffsChanged;

    protected override void Awake()
    {
        storedPlayerUnlockedStats = ScriptableObject.CreateInstance<Stat_SetupSO>();
    }

    [ContextMenu("Add 20 Stat Points")]
    public void TestAddStatPoints() => AddStatPoint(20);

    public void AddStatPoint(int amount)
    {
        UI_FloatingText_Manager.Instance?.CreateFloatingText($"+{amount} Stat Points", transform.position, TextType.Heal);
        statPoints += amount;
        OnStatPointsChanged?.Invoke();
    }

    public bool SpendStatPoint()
    {
        if (statPoints > 0)
        {
            statPoints--;
            OnStatPointsChanged?.Invoke();
            return true;
        }
        return false;
    }

    public float GetUnlockedStatValue(Stat_Type type)
    {
        return storedPlayerUnlockedStats.GetStatValue(type);
    }

    public bool CanApplyBuffOf(string source)
    {
        return activeBuffsList.Exists(buff => buff.source == source) == false;
    }

    public void ApplyTempBuff(Buff_Effect_Data[] buffsToApply, float duration, string source, Sprite icon = null, Item_Effect_DataSO effect = null)
    {
        StartCoroutine(TempBuffCo(buffsToApply, duration, source, icon, effect));
    }

    public void ApplyTempBuff(Buff_Effect_Data[] buffsToApply, float duration, string source)
    {
        StartCoroutine(TempBuffCo(buffsToApply, duration, source, null, null));
    }
 
    private IEnumerator TempBuffCo(Buff_Effect_Data[] buffsToApply, float duration, string source, Sprite icon, Item_Effect_DataSO effect)
    {
        if (icon == null && effect != null && Item_Database.Instance != null && Item_Database.Instance.itemList != null)
        {
            var matchingItem = Item_Database.Instance.itemList.Find(item => item != null && item.itemEffect == effect);
            if (matchingItem != null)
            {
                icon = matchingItem.itemIcon;
            }
        }

        ActiveBuff activeBuffObj = new ActiveBuff
        {
            source = source,
            duration = duration,
            endTime = Time.time + duration,
            icon = icon,
            buffs = buffsToApply
        };

        activeBuffsList.Add(activeBuffObj);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).AddModifier(buff.value, source);
        }

        UI_Manager.Instance.UpdateStatWindow();
        OnBuffsChanged?.Invoke();

        yield return new WaitForSeconds(duration);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).RemoveModifier(source);
        }

        UI_Manager.Instance.UpdateStatWindow();
        activeBuffsList.Remove(activeBuffObj);
        OnBuffsChanged?.Invoke();
    }

    public void AddPersistentStatBonus(Stat_Type type, float value)
    {
        storedPlayerUnlockedStats.UpdateState(type, value);
        AddUnlockedStatToBase(type, value);
        OnUnlockedStatsChanged?.Invoke();
    }

    public void RemovePersistentStatBonus(Stat_Type type, float value)
    {
        storedPlayerUnlockedStats.UpdateState(type, -value);
        RemoveUnlockedStatFromBase(type, value);
        OnUnlockedStatsChanged?.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        data.statPoints = statPoints;
        data.playerUnlockedStats.Clear();

        data.playerUnlockedStats.Add("maxHealth", storedPlayerUnlockedStats.maxHealth);
        data.playerUnlockedStats.Add("healthRegen", storedPlayerUnlockedStats.healthRegen);
        data.playerUnlockedStats.Add("attackSpeed", storedPlayerUnlockedStats.attackSpeed);
        data.playerUnlockedStats.Add("damage", storedPlayerUnlockedStats.damage);
        data.playerUnlockedStats.Add("critChance", storedPlayerUnlockedStats.critChance);
        data.playerUnlockedStats.Add("critPower", storedPlayerUnlockedStats.critPower);
        data.playerUnlockedStats.Add("fireDamage", storedPlayerUnlockedStats.fireDamage);
        data.playerUnlockedStats.Add("iceDamage", storedPlayerUnlockedStats.iceDamage);
        data.playerUnlockedStats.Add("lightningDamage", storedPlayerUnlockedStats.lightningDamage);
        data.playerUnlockedStats.Add("armor", storedPlayerUnlockedStats.armor);
        data.playerUnlockedStats.Add("evasion", storedPlayerUnlockedStats.evasion);
        data.playerUnlockedStats.Add("fireResistance", storedPlayerUnlockedStats.fireResistance);
        data.playerUnlockedStats.Add("iceResistance", storedPlayerUnlockedStats.iceResistance);
        data.playerUnlockedStats.Add("lightningResistance", storedPlayerUnlockedStats.lightningResistance);
        data.playerUnlockedStats.Add("strength", storedPlayerUnlockedStats.strength);
        data.playerUnlockedStats.Add("agility", storedPlayerUnlockedStats.agility);
        data.playerUnlockedStats.Add("intelligence", storedPlayerUnlockedStats.intelligence);
        data.playerUnlockedStats.Add("vitality", storedPlayerUnlockedStats.vitality);
    }

    public void LoadData(GameData data)
    {
        statPoints = data.statPoints;
        data.playerUnlockedStats.TryGetValue("maxHealth", out storedPlayerUnlockedStats.maxHealth);
        data.playerUnlockedStats.TryGetValue("healthRegen", out storedPlayerUnlockedStats.healthRegen);
        data.playerUnlockedStats.TryGetValue("attackSpeed", out storedPlayerUnlockedStats.attackSpeed);
        data.playerUnlockedStats.TryGetValue("damage", out storedPlayerUnlockedStats.damage);
        data.playerUnlockedStats.TryGetValue("critChance", out storedPlayerUnlockedStats.critChance);
        data.playerUnlockedStats.TryGetValue("critPower", out storedPlayerUnlockedStats.critPower);
        data.playerUnlockedStats.TryGetValue("fireDamage", out storedPlayerUnlockedStats.fireDamage);
        data.playerUnlockedStats.TryGetValue("iceDamage", out storedPlayerUnlockedStats.iceDamage);
        data.playerUnlockedStats.TryGetValue("lightningDamage", out storedPlayerUnlockedStats.lightningDamage);
        data.playerUnlockedStats.TryGetValue("armor", out storedPlayerUnlockedStats.armor);
        data.playerUnlockedStats.TryGetValue("evasion", out storedPlayerUnlockedStats.evasion);
        data.playerUnlockedStats.TryGetValue("fireResistance", out storedPlayerUnlockedStats.fireResistance);
        data.playerUnlockedStats.TryGetValue("iceResistance", out storedPlayerUnlockedStats.iceResistance);
        data.playerUnlockedStats.TryGetValue("lightningResistance", out storedPlayerUnlockedStats.lightningResistance);
        data.playerUnlockedStats.TryGetValue("strength", out storedPlayerUnlockedStats.strength);
        data.playerUnlockedStats.TryGetValue("agility", out storedPlayerUnlockedStats.agility);
        data.playerUnlockedStats.TryGetValue("intelligence", out storedPlayerUnlockedStats.intelligence);
        data.playerUnlockedStats.TryGetValue("vitality", out storedPlayerUnlockedStats.vitality);

        AddUnlockedStatToBase(Stat_Type.MaxHealth, storedPlayerUnlockedStats.maxHealth);
        AddUnlockedStatToBase(Stat_Type.HealthRegen, storedPlayerUnlockedStats.healthRegen);
        AddUnlockedStatToBase(Stat_Type.AttackSpeed, storedPlayerUnlockedStats.attackSpeed);
        AddUnlockedStatToBase(Stat_Type.Damage, storedPlayerUnlockedStats.damage);
        AddUnlockedStatToBase(Stat_Type.CritChance, storedPlayerUnlockedStats.critChance);
        AddUnlockedStatToBase(Stat_Type.CritPower, storedPlayerUnlockedStats.critPower);
        AddUnlockedStatToBase(Stat_Type.FireDamage, storedPlayerUnlockedStats.fireDamage);
        AddUnlockedStatToBase(Stat_Type.IceDamage, storedPlayerUnlockedStats.iceDamage);
        AddUnlockedStatToBase(Stat_Type.LightningDamage, storedPlayerUnlockedStats.lightningDamage);
        AddUnlockedStatToBase(Stat_Type.Armor, storedPlayerUnlockedStats.armor);
        AddUnlockedStatToBase(Stat_Type.Evasion, storedPlayerUnlockedStats.evasion);
        AddUnlockedStatToBase(Stat_Type.FireResistance, storedPlayerUnlockedStats.fireResistance);
        AddUnlockedStatToBase(Stat_Type.IceResistance, storedPlayerUnlockedStats.iceResistance);
        AddUnlockedStatToBase(Stat_Type.LightningResistance, storedPlayerUnlockedStats.lightningResistance);
        AddUnlockedStatToBase(Stat_Type.Strength, storedPlayerUnlockedStats.strength);
        AddUnlockedStatToBase(Stat_Type.Agility, storedPlayerUnlockedStats.agility);
        AddUnlockedStatToBase(Stat_Type.Intelligence, storedPlayerUnlockedStats.intelligence);
        AddUnlockedStatToBase(Stat_Type.Vitality, storedPlayerUnlockedStats.vitality);
        OnUnlockedStatsChanged?.Invoke();
    }
}