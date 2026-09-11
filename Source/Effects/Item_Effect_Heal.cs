using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Heal Effect", fileName = "Item Effect Data - Heal")]
public class Item_Effect_Heal : Item_Effect_DataSO
{
    [SerializeField] private float healPercentage = 0.1f;

    public override bool CanBeUsed(Player player)
    {
        if (!base.CanBeUsed(player))
            return false;

        return player.health.GetHealthPercent() < 1f;
    }

    protected override void ActivateEffect()
    {
        float healAmount = player.stats.GetMaxHealth() * healPercentage;
        player.health.IncreaseHealth(healAmount);
    }
}