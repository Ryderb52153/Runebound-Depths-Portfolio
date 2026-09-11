using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Gain Armor Effect", fileName = "Item Effect Data - Gain Armor")]
public class Item_Effect_GainArmor : Item_Effect_OnHealthUpdated
{
    [SerializeField] private float healthPercentThreshold = .35f;
    [SerializeField] private int armorAmount = 10;

    private bool hasGainedArmor = false;
    private Stat armorStat;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        hasGainedArmor = false;
        armorStat = player.stats.GetStatByType(Stat_Type.Armor);
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();

        if(hasGainedArmor)
            armorStat.RemoveModifier(GetType().Name);
    }

    protected override bool MeetsConditions()
    {
        if (!hasGainedArmor && player.health.GetHealthPercent() <= healthPercentThreshold)
            return true;

        if (hasGainedArmor && player.health.GetHealthPercent() > healthPercentThreshold)
            return true;

        return false;
    }

    protected override void ActivateEffect()
    {
        if (!hasGainedArmor)
        {
            armorStat.AddModifier(armorAmount, GetType().Name);
            UI_Manager.Instance.UpdateStatWindow();
            hasGainedArmor = true;
        }
        else
        {
            armorStat.RemoveModifier(GetType().Name);
            UI_Manager.Instance.UpdateStatWindow();
            hasGainedArmor = false;
        }
    }
}