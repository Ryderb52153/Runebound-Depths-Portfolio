using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private float circleAttackCheckRadius = 1;
    [SerializeField] private Vector3 boxAttackCheckRadius = new Vector3(4, 1, 1);
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileDelayTimer = 0;
    public AttackDefinition projectileAttackDefinition;

    [Header("Companion/Minion")]
    [SerializeField] private GameObject companionPrefab;

    public Damage_Scale_Data basicAttackScale;
    public event Action<float, Transform> OnPhysicalDamagePerformed;

    public Entity entity { get; set; }
    public AttackDefinition[] attacks { get; set; }

    private Entity_Stats damageDealerStats;
    
    private void Awake()
    {
        damageDealerStats = GetComponent<Entity_Stats>();
    }

    public List<Collider2D> PerformAttack(AttackDefinition attackDefinition)
    {
        Collider2D[] colliders = GetColliders(attackDefinition.hitbox);
        List<Collider2D> hitTargets = new List<Collider2D>();

        foreach (var target in colliders)
        {
            ITake_Damage targetDamageable = target.GetComponent<ITake_Damage>();

            if (targetDamageable == null)
                continue;

            Attack_Data attackData = new Attack_Data(damageDealerStats, attackDefinition, transform.position);
            ElementType element = attackData.element;

            hitTargets.Add(target);
            targetDamageable.TakeDamage(attackData, attackDefinition.knockback);
            OnPhysicalDamagePerformed?.Invoke(attackData.physicalDamage, target.transform);

            if (element != ElementType.None && target.gameObject.layer != LayerMask.NameToLayer("Echo Player"))
                target.GetComponent<Entity_Status_Handler>().ApplyStatusEffect(element, attackData.elementalEffectData);
        }

        return hitTargets;
    }

    public void SpawnProjectile()
    {
        Projectile projectile = ObjectPoolManager.SpawnObject(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        projectile.Direction = transform.localRotation.y == -1 ? Vector2.left : Vector2.right;
        projectile.AttackData = new Attack_Data(damageDealerStats, projectileAttackDefinition, transform.position);
        projectile.DelayTimer = projectileDelayTimer;
    }

    public void SummonCompanion()
    {
        Vector2 targetSpawnPoint = transform.localRotation.y == -1 ? transform.position + Vector3.left : transform.position + Vector3.right;
        GameObject gameObject = ObjectPoolManager.SpawnObject(companionPrefab, targetSpawnPoint, Quaternion.identity);
        Enemy_Minion minion = gameObject.GetComponent<Enemy_Minion>();
        minion.SetUpMinion();
    }

    protected Collider2D[] GetColliders(AttackCollider attackCollider)
    {
        Vector2 origin = (Vector2)targetCheck.position + GetFacingAdjustedOffset(attackCollider.offset);

        switch (attackCollider.shape)
        {
            case AttackShape.frontCircle:
                return Physics2D.OverlapCircleAll(origin, attackCollider.circleRadius, whatIsTarget);
            case AttackShape.frontBox:
                return Physics2D.OverlapBoxAll(origin, attackCollider.boxSize, attackCollider.angle, whatIsTarget);
            default:
                return Array.Empty<Collider2D>();
        }
    }

    private Vector2 GetFacingAdjustedOffset(Vector2 offset)
    {
        return new Vector2(offset.x * entity.facingDirection, offset.y);
    }

    protected Collider2D[] CircleattackColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, circleAttackCheckRadius, whatIsTarget);
    }

    protected Collider2D[] BoxAttackColliders()
    {
        return Physics2D.OverlapBoxAll(targetCheck.position, boxAttackCheckRadius, 0, whatIsTarget);
    }
}