using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Skill_SwordSlash : Skill_Base
{
    [SerializeField] private float healPercentage = 0.25f;
    [SerializeField] private float cooldownReductionPerEnemy = 1f;
    [SerializeField] private float cleaveRadius = 2f;
    [SerializeField] private AttackDefinition attackDefinition;
    [SerializeField] private LayerMask whatIsTarget;

    private float damageMultiplier = 1f;
    private int attackIndex = 4;

    protected override void Awake()
    {
        base.Awake();
        skillType = Skill_Type.SwordSlash;
        upgradeType = Skill_Upgrade_Type.None;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        List<Collider2D> allHitEnemies = new List<Collider2D>();
        allHitEnemies = player.combat.PerformAttack(player.combat.attacks[attackIndex]);

        if (allHitEnemies.Count <= 0)
            return;

        foreach (var enemy in allHitEnemies)
        {
            UpgradeEffects(enemy);
        }
    }

    private void UpgradeEffects(Collider2D enemyHit)
    {
        if (upgradeType == Skill_Upgrade_Type.SwordSlash_Heal)
        {
            HealOnDamage();
            ReduceAllSkillCooldowns();
        }
        else if (upgradeType == Skill_Upgrade_Type.SwordSlash_Cooldown)
            ReduceAllSkillCooldowns();
        else if (upgradeType == Skill_Upgrade_Type.SwordSlash_Cleave)
            CleaveNearbyOtherEnemies(enemyHit);
    }

    private void CleaveNearbyOtherEnemies(Collider2D enemyHit)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(enemyHit.transform.position, cleaveRadius, whatIsTarget);

        foreach (var enemey in enemies)
        {
            if (enemey.transform == enemyHit.transform)
                continue;

            ITake_Damage cleaveDamageable = enemey.GetComponent<ITake_Damage>();
            if (cleaveDamageable == null) 
                continue;

            Attack_Data cleaveAttackData = new Attack_Data(player.stats, damageScaleData);
            cleaveAttackData.Transform = transform;
            cleaveDamageable.TakeDamage(cleaveAttackData, attackDefinition.knockback);
        }
    }

    private void ReduceAllSkillCooldowns()
    {
        foreach (var skill in skill_Manager.allSkills)
        {
            if (skill != this)
                skill.ReduceCooldownBy(cooldownReductionPerEnemy);
        }
    }

    private void HealOnDamage()
    {
        float physicaldamage = player.stats.GetPhysicalDamage(out bool isCrit);
        int healingAmount = (int)(damageMultiplier * physicaldamage * healPercentage);
        player.health.IncreaseHealth(healingAmount);
    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }
}