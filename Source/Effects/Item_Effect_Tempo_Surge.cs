using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Effect item/Stormcaller Surge", fileName = "Item Effect Data - Stormcaller Surge")]
public class Item_Effect_Tempo_Surge : Item_Effect_OnSkillUsed
{
    [Header("Buff Settings")]
    [SerializeField] private Buff_Effect_Data[] buffs;
    [SerializeField] private float duration = 4f;

    [Header("Optional unique buff source override")]
    [SerializeField] private string customSource;

    private string Source => string.IsNullOrWhiteSpace(customSource) ? GetType().Name : customSource;

    protected override bool MeetsConditions()
    {
        if (player == null || player.stats == null)
            return false;

        return player.stats.CanApplyBuffOf(Source);
    }

    protected override void ActivateEffect()
    {
        player.stats.ApplyTempBuff(buffs, duration, Source, null, this);
    }
}
