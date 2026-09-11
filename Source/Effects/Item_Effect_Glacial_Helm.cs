using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/GlacialHelm", fileName = "Item Effect Data - Glacial Helm")]
public class Item_Effect_Glacial_Helm : Item_Effect_DataSO
{

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        Player.Instance.stats.GetStatByType(Stat_Type.IceDamage).AddMultiplier(0.2f);
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        Player.Instance.stats.GetStatByType(Stat_Type.IceDamage).AddMultiplier(-0.2f);
    }

    protected override void ActivateEffect()
    {
        
    }
}