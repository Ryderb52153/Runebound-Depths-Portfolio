using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    private Inventory_Item quickItem;
    public List<Inventory_Equipment_Slot> equipmentSlots;
    public event Action<Inventory_Item> OnQuickItemChanged;

    public void SetQuickItem(Inventory_Item itemToSet)
    {
        quickItem = itemToSet;
        quickItem.AddItemEffect(player);
        OnQuickItemChanged?.Invoke(quickItem);
    }
    
    public void TryUseQuickItem()
    {
        if (quickItem == null)
            return;

        TryUseItem(quickItem);

        if (FindSameItem(quickItem) == null)
            quickItem = FindSameItem(quickItem);

        OnQuickItemChanged?.Invoke(quickItem);
    }

    public void TryEquipItem(Inventory_Item item)
    {
        if (item.itemData.itemType == ItemType.Material)
            return;

        Inventory_Item inventoryItem = FindItem(item);

        var matchingSlots = equipmentSlots.FindAll(slot => slot.slotType == item.itemData.itemType && slot.equipmentSlot == item.itemData.equipmentSlot);

        // STEP 1: Try to find empty slot and equip there
        foreach (var slot in matchingSlots)
        {
            if(!slot.HasItem())
            {
                EquipItem(inventoryItem, slot);
                RemoveOneItem(inventoryItem);
                return;
            }
        }

        // STEP 2: No empty slot found, replace first occupied slot
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equipedItem;

        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
        RemoveOneItem(inventoryItem);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_Equipment_Slot slot)
    {
        float savedHealthPercent = player.health.GetHealthPercent();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifier(player.stats);
        slot.equipedItem.AddItemEffect(player);

        player.health.SetHealthToPercent(savedHealthPercent);
    }

    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (!CanAddItem(itemToUnequip) && replacingItem == false)
            return;

        float savedHealthPercent = player.health.GetHealthPercent();
        var slotToUnequip = equipmentSlots.Find(slot => slot.equipedItem == itemToUnequip);

        if(slotToUnequip != null)
            slotToUnequip.equipedItem = null;

        itemToUnequip.RemoveModifiers(player.stats);
        itemToUnequip.RemoveItemEffect();

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);
    }

    public override void SaveData(ref GameData data)
    {
        data.inventory.Clear();
        data.equippedItems.Clear();

        foreach (var item in GetInventoryItems())
        {
            if(item != null && item.itemData != null)
            {
                string saveId = item.itemData.saveID;

                if (data.inventory.ContainsKey(saveId) == false)
                    data.inventory[saveId] = 0;

                data.inventory[saveId] += item.stackSize;
            }
        }

        foreach(var slot in equipmentSlots)
        {
            if(slot.HasItem())
                data.equippedItems[slot.equipedItem.itemData.saveID] = slot.slotType;
        }
    }

    public override void LoadData(GameData data)
    {
        foreach(var item in data.inventory)
        {
            string saveId = item.Key;
            int stackSize = item.Value;

            Item_DataSO itemToAdd = itemDatabase.GetItemData(saveId);

            if(itemToAdd == null)
            {
                Debug.LogWarning($"Item with SaveID {saveId} not found in database. Skipping load for this item.");
                continue;
            }

            for(int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemToAdd);
                AddItem(itemToLoad);
            }
        }

        foreach(var entry in data.equippedItems)
        {
            string saveID = entry.Key;
            ItemType slotType = entry.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveID);
            Inventory_Item itemToLoad = new Inventory_Item(itemData);

            var slot = equipmentSlots.Find(s => s.slotType == slotType && s.HasItem() == false && s.equipmentSlot == itemToLoad.itemData.equipmentSlot);

            EquipItem(itemToLoad, slot);
        }

        TriggerUpdatedUI();
    }
}
