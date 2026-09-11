using System;

[Serializable]
public class Elemental_Effect_Data 
{
    public float chillDuration;
    public float chillSlowEffect;

    public float burnDuration;
    public float totalBurnDamae;

    public float shockDuration;
    public float totalShockDamage;
    public float shockCharge;

    public Elemental_Effect_Data(Entity_Stats stats , Damage_Scale_Data damageScale)
    {
        chillDuration = damageScale.chillDuration;
        chillSlowEffect = damageScale.chillSlowMultiplier;

        burnDuration = damageScale.burnDuration;
        totalBurnDamae = stats.offenseStats.fireDamage.GetValue() * damageScale.burnDamageScale;

        shockDuration = damageScale.shockDuration;
        totalShockDamage = stats.offenseStats.lightningDamage.GetValue() * damageScale.shockDamageScale;
        shockCharge = damageScale.shockCharge;
    }
}

