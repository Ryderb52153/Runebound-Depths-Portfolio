using UnityEngine;

public class Skill_Object_Sword : Skill_Object_Base
{
    protected Skill_SwordThrow swordManager;
    protected Transform player;
    protected float returnSpeed = 30;
    protected bool shouldReturn;
    protected float maxAllowedDistance = 20f;

    protected virtual void Update()
    {
        transform.right = rb.linearVelocity.normalized;
        HandleSwordReturn();
    }

    public virtual void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;
        player = swordManager.transform.root;
        this.swordManager = swordManager;
        playerStats = swordManager.player.stats;
        damageScaleData = swordManager.damageScaleData;
    }

    public void ReturnSwordToPlayer() => shouldReturn = true;

    protected void HandleSwordReturn()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if(distance > maxAllowedDistance)
            ReturnSwordToPlayer();

        if (!shouldReturn)
            return;

        transform.position = Vector2.MoveTowards(transform.position, player.position, returnSpeed * Time.deltaTime);

        if(distance < 0.5f)
            ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StopSword(collision);
        Collider2D[] enemies = GetEnemiesAround(transform, 1);
        DamageEnemiesInRadius(enemies);
    }

    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;
        transform.parent = collision.transform;
    }

    private void OnEnable()
    {
        shouldReturn = false;
        rb.simulated = true;
    }
}