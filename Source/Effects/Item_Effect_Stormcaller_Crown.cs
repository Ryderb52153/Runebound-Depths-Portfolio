using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Stormcaller Crown", fileName = "Item Effect Data - Stormcaller Crown")]
public class Item_Effect_Stormcaller_Crown : Item_Effect_DataSO
{
    [Header("Passive Bonuses")]
    [SerializeField] private float lightningDamageMultiplier = 0.2f;
    [SerializeField] private float critChanceBonus = 5f;

    private string Source => GetType().Name;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        playerStats.GetStatByType(Stat_Type.LightningDamage).AddMultiplier(lightningDamageMultiplier);
        playerStats.GetStatByType(Stat_Type.CritChance).AddModifier(critChanceBonus, Source);
    }

    public override void Unsubscribe()
    {
        if (playerStats != null)
        {
            playerStats.GetStatByType(Stat_Type.LightningDamage).AddMultiplier(-lightningDamageMultiplier);
            playerStats.GetStatByType(Stat_Type.CritChance).RemoveModifier(Source);
        }

        base.Unsubscribe();
    }

    protected override void ActivateEffect()
    {

    }
}
