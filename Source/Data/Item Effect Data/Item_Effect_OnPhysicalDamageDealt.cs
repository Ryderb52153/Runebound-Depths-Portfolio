using UnityEngine;

public abstract class Item_Effect_OnPhysicalDamageDealt : Item_Effect_DataSO
{
    protected float pendingDamageDealt;
    protected Transform target;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);

        if (this.player == null)
            return;

        this.player.combat.OnPhysicalDamagePerformed += OnPhysicalDamagePerformed;
    }

    public override void Unsubscribe()
    {
        if (player != null)
            player.combat.OnPhysicalDamagePerformed -= OnPhysicalDamagePerformed;

        base.Unsubscribe();
    }

    private void OnPhysicalDamagePerformed(float damageDealt, Transform target)    
    {
        pendingDamageDealt = damageDealt;
        this.target = target;   
        TryUse();
    }
}