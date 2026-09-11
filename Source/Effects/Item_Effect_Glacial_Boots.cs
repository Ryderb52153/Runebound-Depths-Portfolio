using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Glacial Boots", fileName = "Item Effect Data - Glacial Boots")]
public class Item_Effect_Glacial_Boots : Item_Effect_OnPhysicalDamageDealt
{
    [Header("VFX Obejcts")]
    [SerializeField] private GameObject iceBlastVFXPrefab;

    [Header("Blast Settings")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AttackDefinition attackDefinition;
    [SerializeField] private Attack_Data iceBlastAttackData;
    [SerializeField] private float blastRadius = 2.5f;

    [Header("Buff Settings")]
    [SerializeField] private Stat_Type buffType;
    [SerializeField] private float buffAmount;
    [SerializeField] private float buffDuration;

    private Attack_Data totalDamage;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        totalDamage.element = iceBlastAttackData.element;
        totalDamage.elementalDamage = iceBlastAttackData.elementalDamage;
    }

    protected override bool MeetsConditions()
    {
        if (player == null || pendingDamageDealt <= 0)
            return false;

        // Base chance of 10% plus 0.2% for every point of damage dealt
        // Example: 100 damage hit = 10 + 20 = 30% chance.
        // Example: 250 damage hit = 10 + 50 = 60% chance (max).
        float baseChance = 10f;
        float chancePerDamage = 0.2f;

        float calculatedChance = baseChance + (pendingDamageDealt * chancePerDamage);
        float finalChance = Mathf.Min(calculatedChance, 60f);

        return Random.Range(0f, 100f) < finalChance;
    }

    protected override void ActivateEffect()
    {
        if (iceBlastVFXPrefab != null)
            player.vfx.CreateEffect(iceBlastVFXPrefab, target);

        AddModifiers();
        DamageEnemiesWithIce();
        RemoveModifiers();
        pendingDamageDealt = 0f;
        TempBuff();
    }

    private void TempBuff()
    {
        Player.Instance.stats.ApplyTempBuff(new Buff_Effect_Data[]
        {
            new Buff_Effect_Data
            {
                type = buffType,
                value = buffAmount
            }
        }, buffDuration, GetType().Name, null, this);
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(target.position, blastRadius, enemyMask);

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