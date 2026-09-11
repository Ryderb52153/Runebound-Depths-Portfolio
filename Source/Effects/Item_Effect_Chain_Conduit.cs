using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Stormcaller Conduit", fileName = "Item Effect Data - Stormcaller Conduit")]
public class Item_Effect_Chain_Conduit : Item_Effect_OnPhysicalDamageDealt
{
    [Header("VFX Objects")]
    [SerializeField] private GameObject lightningVFXPrefab;

    [Header("Chain Settings")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AttackDefinition attackDefinition;
    [SerializeField] private Attack_Data lightningAttackData;
    [SerializeField] private float chainRadius = 3.5f;
    [SerializeField] private int maxChainTargets = 3;

    [Header("Proc Chance")]
    [SerializeField] private float baseChance = 10f;
    [SerializeField] private float chancePerDamage = 0.2f;
    [SerializeField] private float maxChance = 60f;

    protected override bool MeetsConditions()
    {
        if (player == null || pendingDamageDealt <= 0 || target == null)
            return false;

        float calculatedChance = baseChance + (pendingDamageDealt * chancePerDamage);
        float finalChance = Mathf.Min(calculatedChance, maxChance);

        return Random.Range(0f, 100f) < finalChance;
    }

    protected override void ActivateEffect()
    {
        // Scale the authored base lightning damage by the player's current Lightning Damage,
        // then revert so the ScriptableObject's serialized base value is preserved.
        float bonus = playerStats.GetAttributeValue(Stat_Type.LightningDamage);
        lightningAttackData.element = ElementType.Lightning;
        lightningAttackData.Transform = player.transform;
        lightningAttackData.elementalDamage += bonus;

        ChainToNearbyEnemies();

        lightningAttackData.elementalDamage -= bonus;
        pendingDamageDealt = 0f;
    }

    private void ChainToNearbyEnemies()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(target.position, chainRadius, enemyMask);

        int chained = 0;
        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (chained >= maxChainTargets)
                break;

            ITake_Damage damagable = enemy.GetComponent<ITake_Damage>();

            if (damagable == null)
                continue;

            if (lightningVFXPrefab != null)
                player.vfx.CreateEffect(lightningVFXPrefab, enemy.transform);

            Entity_Status_Handler statusHandler = enemy.GetComponent<Entity_Status_Handler>();
            damagable.TakeDamage(lightningAttackData, attackDefinition.knockback);
            statusHandler?.ApplyStatusEffect(ElementType.Lightning, lightningAttackData.elementalEffectData);

            chained++;
        }
    }
}
