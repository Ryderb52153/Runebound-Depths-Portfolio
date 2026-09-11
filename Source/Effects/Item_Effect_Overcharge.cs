using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Stormcaller Overcharge", fileName = "Item Effect Data - Stormcaller Overcharge")]
public class Item_Effect_Overcharge : Item_Effect_OnPhysicalDamageDealt
{
    [Header("VFX Objects")]
    [SerializeField] private GameObject lightningVFXPrefab;

    [Header("Cooldown Refund")]
    [SerializeField] private float cooldownRefund = 0.5f;

    [Header("Proc Chance")]
    [SerializeField] private float baseChance = 15f;
    [SerializeField] private float chancePerDamage = 0.1f;
    [SerializeField] private float maxChance = 40f;

    protected override bool MeetsConditions()
    {
        if (player == null || player.skillManager == null || pendingDamageDealt <= 0)
            return false;

        float calculatedChance = baseChance + (pendingDamageDealt * chancePerDamage);
        float finalChance = Mathf.Min(calculatedChance, maxChance);

        return Random.Range(0f, 100f) < finalChance;
    }

    protected override void ActivateEffect()
    {
        if (lightningVFXPrefab != null)
            player.vfx.CreateEffect(lightningVFXPrefab, player.transform);

        player.skillManager.ReduceAllSkillCooldownBy(cooldownRefund);
        pendingDamageDealt = 0f;
    }
}
