using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Ice Blast Effect", fileName = "Item Effect Data - Ice Blast")]
public class Item_Effect_IceBlast : Item_Effect_OnDamageTaken
{
    [Header("VFX Obejcts")]
    [SerializeField] private GameObject iceBlastVFXPrefab;

    [Header("Blast Settings")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AttackDefinition attackDefinition;
    [SerializeField] private Attack_Data iceBlastAttackData;
    [SerializeField] private float healthPercentTrigger = .25f;
    [SerializeField] private float blastRadius = 2.5f;

    private Attack_Data totalDamage;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        totalDamage.element = iceBlastAttackData.element;
        totalDamage.elementalDamage = iceBlastAttackData.elementalDamage;
    }

    protected override bool MeetsConditions()
    {
        if (player == null || player.health == null)
            return false;

        return player.health.GetHealthPercent() <= healthPercentTrigger;
    }

    protected override void ActivateEffect()
    {
        if (iceBlastVFXPrefab != null)
            player.vfx.CreateEffect(iceBlastVFXPrefab, player.transform);

        // Add only player's ice damage and intelligence, not elemental damage total.
        AddModifiers();
        DamageEnemiesWithIce();
        RemoveModifiers();
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, blastRadius, enemyMask);

        foreach (Collider2D target in hitEnemies)
        {
            ITake_Damage damagable = target.GetComponent<ITake_Damage>();

            if (damagable == null)
                continue;

            Entity_Status_Handler statusHandler = target.GetComponent<Entity_Status_Handler>();
            totalDamage.Transform = player.transform; 
            damagable.TakeDamage(totalDamage, attackDefinition.knockback);
            statusHandler?.ApplyStatusEffect(ElementType.Ice, iceBlastAttackData.elementalEffectData);
        }
    }

    private void AddModifiers()
    {
        totalDamage.elementalDamage += Player.Instance.stats.GetAttributeValue(Stat_Type.IceDamage);
    }

    private void RemoveModifiers()
    {
        totalDamage.elementalDamage -= Player.Instance.stats.GetAttributeValue(Stat_Type.IceDamage);
    }
}