using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();  
    
    private float finalValue;
    private bool hasBeenModified = true;

    public float GetBaseValue() => baseValue;
    public float Multiplier { get; set; } = 1f;

    public void SetBaseValue(float value)
    {
        baseValue = value;
        hasBeenModified = true;
    }

    public float GetValue()
    {
        if(hasBeenModified)
        {
            finalValue = GetFinalValue();
            hasBeenModified = false;
        }

        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modifier = new StatModifier(value, source);
        modifiers.Add(modifier);
        hasBeenModified = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(mod => mod.source == source);
        hasBeenModified = true;
    }

    public void AddMultiplier(float multiplier)
    {
        Multiplier += multiplier;
        hasBeenModified = true;
    }

    private float GetFinalValue()
    { 
        float finalValue = baseValue;

        foreach (StatModifier mod in modifiers)
        {
            finalValue += mod.value;
        }

        return finalValue;
    }
}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}