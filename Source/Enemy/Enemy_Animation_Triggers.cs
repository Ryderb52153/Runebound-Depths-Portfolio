using UnityEngine;

public class Enemy_Animation_Triggers : Entity_Animation_Triggers
{
    private Enemy enemy;
    private Enemy_VFX enemy_VFX;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemy_VFX = GetComponentInParent<Enemy_VFX>();
    }

    // Animation Event
    private void EnableCounterWindow()
    {
        enemy.EnableCounterWindow(true);
        enemy_VFX.EnableAttackAlert(true);
    }

    // Animation Event
    private void DisableCounterWindow()
    {
        enemy.EnableCounterWindow(false);
        enemy_VFX.EnableAttackAlert(false);
    }

    // Animation Event
    private void SummonMinion()
    {
        combat.SummonCompanion();
    }

    private void OnEnable()
    {
        DisableCounterWindow();
    }

    private void OnDisable()
    {
        DisableCounterWindow();
    }
}