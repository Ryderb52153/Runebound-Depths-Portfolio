using UnityEngine;
using System.Collections.Generic;

public class Skill_Object_SwordBounce : Skill_Object_Sword
{
    [SerializeField] private float bounceSpeed = 15f;

    private int bounceCount;
    private Collider2D[] enemyTargets;
    private Transform nextTarget;
    private List<Transform> selectedBefore = new List<Transform>();

    public override void SetupSword(Skill_SwordThrow swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        anim.SetTrigger("swordSpin");
        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        HandleSwordReturn();
        HandleBounce();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)
        {
            enemyTargets = GetEnemiesAround(transform, 10);
            rb.simulated = false;
        }

        Collider2D[] enemies = GetEnemiesAround(transform, 1);
        DamageEnemiesInRadius(enemies);

        if(enemyTargets.Length <= 1 || bounceCount <= 0)
            ReturnSwordToPlayer();
        else
            nextTarget = GetNextTarget();
    }

    private void HandleBounce()
    {
        if (nextTarget == null)
            return;

        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.5f)
        {
            Collider2D[] enemies = GetEnemiesAround(transform, 1);
            DamageEnemiesInRadius(enemies);
            BounceToNextTarget();


            if(bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                ReturnSwordToPlayer();
            }
        }
    }

    private void BounceToNextTarget()
    {
        nextTarget = GetNextTarget();
        bounceCount--;
    }

    private Transform GetNextTarget()
    {
        List<Transform> validTargets = GetValidTargets();

        int randomInex = Random.Range(0, validTargets.Count);

        Transform nextTarget = validTargets[randomInex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }

    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new List<Transform>();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach(var enemy in aliveTargets)
        {
            if(enemy != null && selectedBefore.Contains(enemy.transform) == false)
                validTargets.Add(enemy.transform);
        }

        if (validTargets.Count > 0)
            return validTargets;
        else
        {
            selectedBefore.Clear();
            return aliveTargets;
        }
        
    }

    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }

        return aliveTargets;
    }
}