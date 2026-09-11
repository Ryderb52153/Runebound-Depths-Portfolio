using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Equipment item", fileName = "Equipment Data - ")]
public class Equipment_DataSO : Item_DataSO
{
    [Header("Item Modifiers")]
    [SerializeField] private Item_Modifier[] itemModifiers;

    public Item_Modifier[] GetItemModifiers() => itemModifiers;
}

[Serializable]
public class Item_Modifier
{
    public Stat_Type statType;
    public float value;
}