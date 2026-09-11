using System.Collections.Generic;
using UnityEngine;

public class Skill_Domain_Expansion : Skill_Base
{
    [SerializeField] private Skill_Object_DomainExpansion domainPrefab;

    [Header("Slowing Down Upgrade")]
    [SerializeField] private float slowDownPercent = 0.8f;
    [SerializeField] private float slowDownDomainDuration = 5;

    [Header("Spell Casting Upgrade")]
    [SerializeField] private int spellsToCast = 10;
    [SerializeField] private float spellCastingDuration = 8;
    [SerializeField] private float spellCastingSlowDown = 1;
    private float spellCastTimer;
    private float spellsCastPerSecond;

    [Header("Domain details")]
    [SerializeField] private float maxDomainSize = 10f;
    [SerializeField] private float expandSpeed = 3;

    private List<Enemy> trappedTargets = new List<Enemy>();
    private Transform currentTarget;

    protected override void Awake()
    {
        base.Awake();
        skillType = Skill_Type.DomainExpansion;
    }

    public override void UseSkill()
    {
        base.UseSkill();
        CreateDomain();
    }

    private void CreateDomain()
    {
        spellsCastPerSecond = spellsToCast / spellCastingDuration;
        Skill_Object_DomainExpansion domain = ObjectPoolManager.SpawnObject<Skill_Object_DomainExpansion>(domainPrefab, transform.position, Quaternion.identity);
        domain.SetupDomain(this);
    }

    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;

        if(currentTarget == null)
            currentTarget = FindTargetInDomain();

        if (currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);
            spellCastTimer = 1f / spellsCastPerSecond;
            currentTarget = null;
        }
    }

    private void CastSpell(Transform target)
    {
        if(Unlocked(Skill_Upgrade_Type.Domain_EchoSpam))
        {
            Vector3 offset = Random.value < 0.5f ? Vector3.right : Vector3.left;
            skill_Manager.timeEcho.CreateTimeEcho(target.position + offset);
        }
        else if(Unlocked(Skill_Upgrade_Type.Domain_ShardSpam))
        {
            skill_Manager.shard.CreateRawShard(target);
        }
    }

    private Transform FindTargetInDomain()
    {
        trappedTargets.RemoveAll(target => target == null || target.health.isDead);

        if(trappedTargets.Count == 0)
            return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);
        return trappedTargets[randomIndex].transform;
    }

    public float GetMaxDomainSize()
    {
        return maxDomainSize;
    }

    public float GetExpandSpeed()
    {
        return expandSpeed;
    }

    public float GetDomainDuration()
    {
        if(Unlocked(Skill_Upgrade_Type.Domain_SlowingDown))
            return slowDownDomainDuration;
        else
            return spellCastingDuration;
    }

    public float GetSlowPercentage()
    {
        if(Unlocked(Skill_Upgrade_Type.Domain_SlowingDown))
            return slowDownPercent;
        else
            return spellCastingSlowDown;
    }

    public bool JustDomain()
    {
        return upgradeType != Skill_Upgrade_Type.Domain_EchoSpam
            && upgradeType != Skill_Upgrade_Type.Domain_ShardSpam;
    }

    public void AddTarget(Enemy enemy)
    {
        if(!trappedTargets.Contains(enemy))
            trappedTargets.Add(enemy);
    }

    public void ClearAllTargets()
    {
        foreach(Enemy enemy in trappedTargets)
            enemy.StopSlowDown();
        
        trappedTargets.Clear();
    }
}
