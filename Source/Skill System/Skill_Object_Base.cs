using UnityEngine;

public class Skill_Object_Base : MonoBehaviour
{
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1;
    [SerializeField] private AttackDefinition attackDefinition;

    protected Entity_Stats playerStats;
    protected Damage_Scale_Data damageScaleData;
    protected ElementType usedElement;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Transform lastTarget;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected Collider2D[] GetEnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, targetLayerMask);
    }

    protected Transform ClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in GetEnemiesAround(targetCheck, 10))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy.transform;
            }
        }

        return target;
    }

    protected void DamageEnemiesInRadius(Collider2D[] enemies)
    {
        foreach (var target in enemies)
        {
            ITake_Damage damageable = target.GetComponent<ITake_Damage>();

            if (damageable == null)
                continue;

            Attack_Data attackData = new Attack_Data(playerStats, damageScaleData);
            attackData.Transform = transform;
            ElementType element = attackData.element;
            usedElement = element;

            lastTarget = target.transform;
            damageable.TakeDamage(attackData, attackDefinition.knockback);
            

            if (element != ElementType.None)
                target.GetComponent<Entity_Status_Handler>().ApplyStatusEffect(element, attackData.elementalEffectData);
        }
    }

    private void OnDrawGizmos()
    {
        if(targetCheck == null)
            targetCheck = transform;

        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}