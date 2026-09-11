
public abstract class Item_Effect_OnDamageTaken : Item_Effect_DataSO
{
    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.health.OnTakenDamage += OnDamageTaken;
    }

    public override void Unsubscribe()
    {
        if (player != null)
            player.health.OnTakenDamage -= OnDamageTaken;

        base.Unsubscribe();
    }

    protected virtual void OnDamageTaken(Attack_Data attackData)
    {
        TryUse();
    }
}
