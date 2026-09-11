using System;
using UnityEngine;

public class Skill_Object_Shard : Skill_Object_Base
{
    [SerializeField] private GameObject vfxPrefab;

    private Skill_Shard shardManager;
    private Transform target;
    private float speed;

    private void Update()
    {
        if (target == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void SetAttackTarget(float speed, Transform target = null)
    {
        if(target == null)
            this.target = ClosestTarget();
        else
            this.target = target;

        this.speed = speed;
    }

    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;
        Invoke(nameof(Explode), shardManager.GetDetonateTime());
    }

    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;
        Invoke(nameof(Explode), detonationTime);

        if (canMove)
            SetAttackTarget(shardSpeed);
    }

    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed, Transform target)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.player.stats;
        damageScaleData = shardManager.damageScaleData;
        Invoke(nameof(Explode), detonationTime);

        if (canMove)
            SetAttackTarget(shardSpeed, target);
    }

    public void Explode()
    {
        CancelInvoke(nameof(Explode));

        Collider2D[] enemies = GetEnemiesAround(transform, checkRadius);
        DamageEnemiesInRadius(enemies);

        GameObject vfx = ObjectPoolManager.SpawnObject(vfxPrefab, transform.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.player.vfx.GetElementColor(usedElement);
        shardManager.ResetCurrentShard(this);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() == null)
            return;

        Explode();
    }
}