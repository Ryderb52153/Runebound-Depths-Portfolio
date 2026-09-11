using UnityEngine;

public class Skill_Object_SwordSpin : Skill_Object_Sword
{
    private int maxDistance;
    private float attacksPerSecond;
    private float attackTimer;

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        this.maxDistance = swordManager.maxDistance;
        this.attacksPerSecond = swordManager.attacksPerSecond;

        anim?.SetTrigger("swordSpin");
        Invoke(nameof(ReturnSwordToPlayer), swordManager.maxSpinDuration);
    }

    protected override void Update()
    {
        HandleAttack();
        CheckToStop();
        HandleSwordReturn();
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if(attackTimer <= 0)
        {
            Collider2D[] enemies = GetEnemiesAround(transform, 2f);
            DamageEnemiesInRadius(enemies);
            attackTimer = 1f / attacksPerSecond;
        }
    }

    private void CheckToStop()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, swordManager.player.transform.position);

        if(distanceToPlayer >= maxDistance && rb.simulated == true)
            rb.simulated = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
    }
}
