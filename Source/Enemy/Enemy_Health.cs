using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy;
    private int playerLayer;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public override void TakeDamage(Attack_Data attackData, KnockbackData knockbackData)
    {
        if(!canTakeDamage)
            return;

        base.TakeDamage(attackData, knockbackData);

        if (attackData.Transform.gameObject.layer == playerLayer)
        {
            //enemy.TryEnterBattleState(attackData.Transform);
        }
    }
}