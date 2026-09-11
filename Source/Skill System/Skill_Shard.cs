using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    [SerializeField] private Skill_Object_Shard shardPrefab;
    [SerializeField] private float detonationTime = 2f;

    [Header("Moving Shard Upgrade")]
    private float shardSpeed = 7f;

    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges = 3;
    [SerializeField] private bool isRecharging;

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10;

    [Header("Health Reqind Shard Upgrade")]
    [SerializeField] private float savedHealthPercentage;

    private Skill_Object_Shard currentShard;
    private Entity_Health playerHealth;
    

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = GetComponentInParent<Entity_Health>();
        skillType = Skill_Type.TimeShard;
    }

    public Skill_Object_Shard CreateShard()
    {
        Skill_Object_Shard shard = ObjectPoolManager.SpawnObject(shardPrefab, transform.position, Quaternion.identity);
        shard.SetupShard(this);
        return shard;
    }

    public void CreateRawShard()
    {
        bool canMove = Unlocked(Skill_Upgrade_Type.Shard_MoveToEnemy) || Unlocked(Skill_Upgrade_Type.Shard_MultiCast);
        Skill_Object_Shard shard = ObjectPoolManager.SpawnObject(shardPrefab, transform.position, Quaternion.identity);
        shard.SetupShard(this, detonationTime, canMove, shardSpeed);
    }

    public void CreateRawShard(Transform target)
    {
        bool canMove = Unlocked(Skill_Upgrade_Type.Shard_MoveToEnemy) || Unlocked(Skill_Upgrade_Type.Shard_MultiCast);
        Skill_Object_Shard shard = ObjectPoolManager.SpawnObject(shardPrefab, transform.position, Quaternion.identity);
        shard.SetupShard(this, detonationTime, canMove, shardSpeed, target);
    }

    public override void UseSkill()
    {
        if (Unlocked(Skill_Upgrade_Type.Shard))
            SpawnRegularShard();
        else if (Unlocked(Skill_Upgrade_Type.Shard_MoveToEnemy))
            SpawnMovingShard();
        else if (Unlocked(Skill_Upgrade_Type.Shard_MultiCast))
            HandleShardMultiCast();
        else if(Unlocked(Skill_Upgrade_Type.Shard_Teleport))
            HandleShardTeleport();
        else if(Unlocked(Skill_Upgrade_Type.Shard_TeleportHPRewind))
            HandleShardHealthRewind();
    }

    private void SpawnRegularShard()
    {
        CreateShard();
        base.UseSkill();
    }

    private void SpawnMovingShard()
    {
        Skill_Object_Shard shard = CreateShard();
        shard.SetAttackTarget(shardSpeed);
        base.UseSkill();
    }

    private void HandleShardMultiCast()
    {
        if (currentCharges <= 0)
            return;

        Skill_Object_Shard shard = CreateShard();
        shard.SetAttackTarget(shardSpeed);
        currentCharges--;
        
        if(currentCharges == 0)
            base.UseSkill();

        if (!isRecharging)
            StartCoroutine(ShardRechargeCo());
    }

    private void HandleShardTeleport()
    {
        if (currentShard == null)
        {
            currentShard = CreateShard();
        }
        else
        {
            SwapPlayerAndShard();
            currentShard.Explode();
            base.UseSkill();
        }
    }

    private void HandleShardHealthRewind()
    {
        if (currentShard == null)
        {
            currentShard = CreateShard();
            savedHealthPercentage = playerHealth.GetHealthPercent();
        }
        else
        {
            SwapPlayerAndShard();
            playerHealth.SetHealthToPercent(savedHealthPercentage);
            currentShard.Explode();
            base.UseSkill();
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = player.transform.position;

        currentShard.transform.position = playerPosition;
        player.TeleportPlayer(shardPosition);
    }

    private IEnumerator ShardRechargeCo()
    {
        isRecharging = true;

        while (currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            currentCharges++;
        }

        isRecharging = false;
    }

    public void ResetCurrentShard(Skill_Object_Shard shard)
    {
        if(currentShard == shard)
            currentShard = null;
    }

    public float GetDetonateTime()
    {
        if(Unlocked(Skill_Upgrade_Type.Shard_Teleport) || Unlocked(Skill_Upgrade_Type.Shard_TeleportHPRewind))
        {
            return shardExistDuration;
        }

        return detonationTime;
    }
}