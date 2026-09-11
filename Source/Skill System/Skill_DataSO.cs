using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data")]
public class Skill_DataSO : ScriptableObject
{
    [Header("Skill Description")]
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Unlock & Upgrade")]
    public int cost;
    public Skill_Type skillType;
    public UpgradeData upgrade;
}

[Serializable]
public class UpgradeData
{
    public Skill_Upgrade_Type upgradeType;
    public float cooldown;
    public Damage_Scale_Data damageScaleData;
}
