using UnityEngine;

public class Skill_Object_SwordPierce : Skill_Object_Sword
{
    private int amountToPierce;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        amountToPierce = swordManager.pieceSwordCount;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
        Collider2D[] enemies = GetEnemiesAround(transform, .3f);

        if (amountToPierce <= 0 || groundHit)
        {
            DamageEnemiesInRadius(enemies);
            StopSword(collision);
            return;
        }

        amountToPierce--;
        DamageEnemiesInRadius(enemies);
    }
}