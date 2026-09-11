using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/LifeSteal Effect", fileName = "Item Effect Data - LifeSteal")]
public class Item_Effect_LifeSteal : Item_Effect_OnPhysicalDamageDealt
{
    [SerializeField] private float lifeStealPercentage = 0.1f;

    protected override bool MeetsConditions()
    {
        return pendingDamageDealt > 0f;
    }

    protected override void ActivateEffect()
    {
        float healAmount = pendingDamageDealt * lifeStealPercentage;
        player.health.IncreaseHealth(Mathf.RoundToInt(healAmount));
        pendingDamageDealt = 0f;
    }
}