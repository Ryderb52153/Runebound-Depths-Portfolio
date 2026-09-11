using System;
using System.Collections.Generic;
using UnityEngine;

// Potentional change for this class, have a dictionary thats maps skill types to skill instances
// This would make it easier to add new skills without modifying this class
// though it would add some complexity to the initialization process
// For now, keeping it simple and explicit
// Dictionary way would be used so we don't have to specifically call out each skill in each state. and just call start effect for all skills
// Current problem is that scripts that are accessing the skills on here need to know each specific skill type, not the base.

public class Player_Skill_Manager : MonoBehaviour
{
    [SerializeField] private List<Skill_DataSO> skillDatas;
    [SerializeField] private Skill_Dash dashField;
    [SerializeField] private Skill_Shard shardField;
    [SerializeField] private Skill_TimeEcho timeEchoField;
    [SerializeField] private Skill_SwordSlash swordSlashField;
    [SerializeField] private Skill_Domain_Expansion domainExpansionField;

    [Header("Passive Status Buff")]
    [SerializeField] private int passiveBuffLevel;

    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }
    public Skill_SwordSlash swordSlash { get; private set; }
    public Skill_Domain_Expansion domainExpansion { get; private set; }

    public Skill_Base[] allSkills { get; private set; }
    private Dictionary<(Skill_Type, Skill_Upgrade_Type), Skill_DataSO> allSkillDatas;

    private Skill_Base skill_one;
    private Skill_Base skill_two;
    private Skill_Base skill_three;
    private Skill_Base skill_four;
    
    public event Action<Skill_DataSO, Skill_Base> OnSkillUnlocked;

    private void Awake()
    {
        dash = dashField;
        shard = shardField;
        timeEcho = timeEchoField;
        swordSlash = swordSlashField;
        domainExpansion = domainExpansionField;

        allSkills = GetComponentsInChildren<Skill_Base>();

        InitializeSkillDictionary();
    }

    public void UnlockSkill(Skill_DataSO skillData)
    {
        Skill_Type skillType = skillData.skillType;
        Skill_Base skill = GetSkillByType(skillType);

        skill.SetSkillUpgrade(skillData);
        OnSkillUnlocked?.Invoke(skillData, skill);
        SetSkillToAccessor(skillType);
    }

    public void UseSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 1:
                skill_one?.UseSkill();
                break;
            case 2:
                skill_two?.UseSkill();
                break;
            case 3:
                skill_three?.UseSkill();
                break;
            case 4:
                skill_four?.UseSkill();
                break;
        }
    }

    public bool CanUseSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 1:
                return skill_one != null && skill_one.CanUseSkill();
            case 2:
                return skill_two != null && skill_two.CanUseSkill();
            case 3:
                return skill_three != null && skill_three.CanUseSkill();
            case 4:
                return skill_four != null && skill_four.CanUseSkill();
            default:
                return false;
        }
    }

    public void ReduceAllSkillCooldownBy(float amount)
    {
        foreach (var skill in allSkills)
        {
            skill.ReduceCooldownBy(amount);
        }
    }

    public Skill_Base GetSkillByType(Skill_Type type)
    {
        return type switch
        {
            Skill_Type.Dash => dash,
            Skill_Type.TimeShard => shard,
            Skill_Type.TimeEcho => timeEcho,
            Skill_Type.SwordSlash => swordSlash,
            Skill_Type.DomainExpansion => domainExpansion,
            _ => null,
        };
    }

    private void SetSkillToAccessor(Skill_Type skillType)
    {
        switch (skillType)
        {
            case Skill_Type.Dash:
                skill_one = GetSkillByType(skillType);
                break;
            case Skill_Type.TimeEcho:
            case Skill_Type.TimeShard:
                skill_two = GetSkillByType(skillType);
                break;
            case Skill_Type.SwordSlash:
                skill_three = GetSkillByType(skillType);
                break;
            case Skill_Type.DomainExpansion:
                skill_four = GetSkillByType(skillType);
                break;
        }
    }

    private void InitializeSkillDictionary()
    {
        allSkillDatas = new Dictionary<(Skill_Type, Skill_Upgrade_Type), Skill_DataSO>();
        foreach (var skillData in skillDatas)
        {
            var key = (skillData.skillType, skillData.upgrade.upgradeType);
            allSkillDatas[key] = skillData;
        }
    }

    public Skill_DataSO GetSkillData(Skill_Type skillType, Skill_Upgrade_Type upgradeType)
    {
        var key = (skillType, upgradeType);
        if (allSkillDatas.TryGetValue(key, out Skill_DataSO skillData))
        {
            return skillData;
        }

        Debug.Log("Skill data not found for type: " + skillType + " and upgrade: " + upgradeType);
        return null;
    }

    public int PassiveBuffLevel => passiveBuffLevel;

    public void SetPassiveBuffLevel(int level)
    {
        passiveBuffLevel = level;
    }

    public float GetPassiveSkillMultiplier()
    {
        return 1f + (passiveBuffLevel * 0.01f); // 1% per level
    }

    public bool HasUnlockedFourSkills()
    {
        return skill_one != null && skill_two != null && skill_three != null && skill_four != null;
    }
}