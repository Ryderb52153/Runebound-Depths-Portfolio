using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Stormcaller Discharge", fileName = "Item Effect Data - Stormcaller Discharge")]
public class Item_Effect_Static_Discharge : Item_Effect_OnDamageTaken
{
    [Header("VFX Objects")]
    [SerializeField] private GameObject lightningVFXPrefab;

    [Header("Blast Settings")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AttackDefinition attackDefinition;
    [SerializeField] private Attack_Data lightningAttackData;
    [SerializeField] private float blastRadius = 3f;

    protected override void ActivateEffect()
    {
        if (lightningVFXPrefab != null)
            player.vfx.CreateEffect(lightningVFXPrefab, player.transform);

        // Scale the authored base lightning damage by the player's current Lightning Damage,
        // then revert so the ScriptableObject's serialized base value is preserved.
        float bonus = playerStats.GetAttributeValue(Stat_Type.LightningDamage);
        lightningAttackData.element = ElementType.Lightning;
        lightningAttackData.Transform = player.transform;
        lightningAttackData.elementalDamage += bonus;

        DischargeNearbyEnemies();

        lightningAttackData.elementalDamage -= bonus;
    }

    private void DischargeNearbyEnemies()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, blastRadius, enemyMask);

        foreach (Collider2D target in hitEnemies)
        {
            ITake_Damage damagable = target.GetComponent<ITake_Damage>();

            if (damagable == null)
                continue;

            Entity_Status_Handler statusHandler = target.GetComponent<Entity_Status_Handler>();
            damagable.TakeDamage(lightningAttackData, attackDefinition.knockback);
            statusHandler?.ApplyStatusEffect(ElementType.Lightning, lightningAttackData.elementalEffectData);
        }
    }
}
