using System;
using UnityEngine;

[Serializable]
public class Inventory_Equipment_Slot
{
    public ItemType slotType;
    public EquipmentSlot equipmentSlot;
    public Inventory_Item equipedItem;

    public bool HasItem() => equipedItem != null && equipedItem.itemData != null;
}