using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Buff Effect", fileName = "Item Effect Data - Buff")]
public class Item_Effect_Buff : Item_Effect_DataSO
{
    [SerializeField] private Buff_Effect_Data[] buffs;
    [SerializeField] private float duration;

    [Header("Optional unique buff source override")]
    [SerializeField] private string customSource;

    private string Source => string.IsNullOrWhiteSpace(customSource) ? name : customSource;

    public override bool CanBeUsed(Player player)
    {
        if (!base.CanBeUsed(player))
            return false;

        return player.stats.CanApplyBuffOf(Source);
    }

    protected override void ActivateEffect()
    {
        player.stats.ApplyTempBuff(buffs, duration, Source, null, this);
    }
}