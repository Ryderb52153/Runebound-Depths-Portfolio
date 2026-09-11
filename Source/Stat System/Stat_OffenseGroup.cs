using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup 
{
    // Physical Stats
    public Stat damage;
    public Stat attackSpeed;
    public Stat critPower;
    public Stat critChance;
    public Stat armorReduction;

    // Elemental Stats
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
}