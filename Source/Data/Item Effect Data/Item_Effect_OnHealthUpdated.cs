public abstract class Item_Effect_OnHealthUpdated : Item_Effect_DataSO
{
    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.health.OnHealthUpdated += OnHealthUpdated;
    }

    public override void Unsubscribe()
    {
        if (player != null)
            player.health.OnHealthUpdated -= OnHealthUpdated;

        base.Unsubscribe();
    }

    protected virtual void OnHealthUpdated()
    {
        TryUse();
    }
}