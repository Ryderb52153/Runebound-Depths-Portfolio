using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/GlacialGloves", fileName = "Item Effect Data - Glacial Gloves")]
public class Item_Effect_Glacial_Gloves : Item_Effect_OnHealthUpdated
{
    [SerializeField] private float healthPercentThreshold = .35f;
    [SerializeField] private int healthRegenAmount = 20;
    [SerializeField] private float healthRegenDuration = 5f;

    protected override bool MeetsConditions()
    {
        if (player.health.GetHealthPercent() <= healthPercentThreshold)
            return true;

        return false;
    }

    protected override void ActivateEffect()
    {
        Player.Instance.stats.ApplyTempBuff(new Buff_Effect_Data[]
        {
            new Buff_Effect_Data
            {
                type = Stat_Type.HealthRegen,
                value = healthRegenAmount
            }
        }, healthRegenDuration, GetType().Name, null, this);
    }
}