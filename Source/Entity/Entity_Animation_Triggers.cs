using UnityEngine;

public class Entity_Animation_Triggers : MonoBehaviour
{
    [Header("Attack Definitions")]
    [SerializeField] private AttackDefinition[] attacks;


    [Header("Gizmo Preview")]
    [SerializeField] private bool showAttackGizmos = true;
    [SerializeField] private int previewAttackIndex = 0;

    protected Entity entity;
    protected Entity_Combat combat;

    protected virtual void Awake()
    {
        combat = GetComponentInParent<Entity_Combat>();
        entity = GetComponentInParent<Entity>();
        combat.entity = entity;
        combat.attacks = attacks;
    }

    // This method is called via Animation Event
    private void CurrentStateTrigger()
    {
        entity.CurrentStateAnimationTrigger();
    }

    // This method is called via Animation Event
    private void SpawnProjectileTrigger()
    {
        combat.SpawnProjectile();
    }

    // This method is called via Animation Event
    protected virtual void AttackTrigger(int index)
    {
        if (index < 0 || index >= attacks.Length)
            return;

         combat.PerformAttack(attacks[index]);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showAttackGizmos || attacks == null || attacks.Length == 0)
            return;

        if (previewAttackIndex < 0 || previewAttackIndex >= attacks.Length)
            return;

        Transform root = transform.parent;
        if (root == null)
            return;

        AttackCollider attack = attacks[previewAttackIndex].hitbox;

        int facingDirection = 1;
        Entity parentEntity = GetComponentInParent<Entity>();
        if (parentEntity != null)
            facingDirection = parentEntity.facingDirection;

        Vector2 adjustedOffset = new Vector2(attack.offset.x * facingDirection, attack.offset.y);
        Vector3 origin = root.position + (Vector3)adjustedOffset;

        Gizmos.color = Color.red;

        switch (attack.shape)
        {
            case AttackShape.frontCircle:
                Gizmos.DrawWireSphere(origin, attack.circleRadius);
                break;

            case AttackShape.frontBox:
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.Euler(0, 0, attack.angle), Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, attack.boxSize);
                Gizmos.matrix = oldMatrix;
                break;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(root.position, origin);
        Gizmos.DrawSphere(origin, 0.05f);
    }
}

public enum AttackShape
{
    frontCircle,
    frontBox
}

public enum DamageType 
{ 
    Physical, 
    Elemental, 
    Both 
}

[System.Serializable]
public class AttackDefinition
{
    public string attackName;
    public DamageType damageType; // NEW
    public ElementType elementOverride = ElementType.None; // NEW (Optional)
    public bool vfxOverride = false;
    public Damage_Scale_Data damageScale;
    public AttackCollider hitbox;
    public KnockbackData knockback;
}

[System.Serializable]
public class KnockbackData
{
    public bool useKnockback;
    public Vector2 force;
    public float duration = 0.2f;
}

[System.Serializable]
public class AttackCollider
{
    public AttackShape shape;
    public Vector2 offset;
    public float circleRadius = 1f;
    public Vector2 boxSize = Vector2.one;
    public float angle = 0f;
}