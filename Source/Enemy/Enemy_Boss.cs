using System;
using UnityEngine;
using System.Collections;

public abstract class Enemy_Boss : Enemy
{
    public static event Action OnBossFightEnded;

    public void StartBossFight(Transform player)
    {
        anim.SetBool("idle", false);
        this.player = player;
        stateMachine.Initialize(BattleState);
    }

    public void EntrySequence()
    {
        StartCoroutine(StartEntrySequence());
    }

    private IEnumerator StartEntrySequence()
    {
        anim.SetBool("entry", true);
        // Wait for the entry animation to complete
        yield return new WaitForSeconds(2.625f);

        anim.SetBool("entry", false);
        anim.SetBool("idle", true);
    }

    public override void EntityDeath()
    {
        base.EntityDeath();
        OnBossFightEnded?.Invoke();
    }
}
