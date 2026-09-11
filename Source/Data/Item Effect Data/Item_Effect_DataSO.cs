using UnityEngine;

public abstract class Item_Effect_DataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription = "Base Item Effect Description";

    [Header("Common Settings")]
    [SerializeField] protected float cooldown = 0f;

    protected Player player;
    protected Entity_Stats playerStats;

    protected float lastTimeUsed = -999f;

    public virtual bool CanBeUsed(Player player)
    {
        this.player = player;
        playerStats = player != null ? player.stats : null;
        return player != null;
    }

    public virtual void Subscribe(Player player)
    {
        this.player = player;
        playerStats = player.stats;
        lastTimeUsed = 0;
    }

    public virtual void Unsubscribe()
    {
        player = null;
        playerStats = null;
    }

    // For Active item effects
    // answers "can it fire right now?"
    public bool Use(Player player)
    {
        if (!CanBeUsed(player))
            return false;

        return TryUse();
    }

    // For passive/event item effects
    // answers "is it valid at all?"
    protected bool TryUse()
    {
        if (player == null)
            return false;

        if (!HasCooldownReady())
            return false;

        if (!MeetsConditions())
            return false;

        ActivateEffect();
        MarkUsed();
        return true;
    }

    protected bool HasCooldownReady()
    {
        return Time.time >= lastTimeUsed + cooldown;
    }

    protected void MarkUsed()
    {
        lastTimeUsed = Time.time;
    }

    protected virtual bool MeetsConditions()
    {
        return true;
    }

    protected abstract void ActivateEffect();
}