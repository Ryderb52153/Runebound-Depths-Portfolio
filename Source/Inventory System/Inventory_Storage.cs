using System.Collections.Generic;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    private Inventory_Player inventoryPlayer;
    private List<Inventory_Item> materialStash = new List<Inventory_Item>();


    public List<Inventory_Item> GetMaterialStash() => materialStash;
    public void SetInventory(Inventory_Player inventory) => this.inventoryPlayer = inventory;

    public void FromPlayerToStorage(Inventory_Item item)
    {
        if (!CanAddItem(item))
            return;

        var itemToAdd = new Inventory_Item(item.itemData);

        if (item.itemData.itemType == ItemType.Material)
            AddMaterialToStash(itemToAdd);
        else
            AddItem(itemToAdd);

        inventoryPlayer.RemoveOneItem(item);
        TriggerUpdatedUI();
    }

    public void FromStorageToPlayer(Inventory_Item item)
    {
        if (!inventoryPlayer.CanAddItem(item))
            return;

        var itemToAdd = new Inventory_Item(item.itemData);
        inventoryPlayer.AddItem(itemToAdd);

        if (item.itemData.itemType == ItemType.Material)
            RemoveOneItemFromStash(item);
        else
            RemoveOneItem(item);

        TriggerUpdatedUI();
    }

    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        var stackableItem = StackableInStash(itemToAdd);

        if(stackableItem != null)
            stackableItem.AddToStack(itemToAdd.stackSize);
        else
            materialStash.Add(itemToAdd);

        TriggerUpdatedUI();
    }

    public Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanStackMore());
    }

    private void RemoveOneItemFromStash(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = materialStash.Find(item => item == itemToRemove);

        if (itemInInventory.stackSize > 1)
            itemInInventory.RemoveFromStack(1);
        else
            materialStash.Remove(itemToRemove);
    }

    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);

        data.storageItems.Clear();
        data.materialStash.Clear();

        foreach (var item in GetInventoryItems())
        {
            if (item == null || item.itemData == null)
            {
                continue;
            }

            string saveId = item.itemData.saveID;

            if (data.storageItems.ContainsKey(saveId) == false)
                data.storageItems[saveId] = 0;

            data.storageItems[saveId] += item.stackSize;
        }

        foreach (var item in GetMaterialStash())
        {
            if (item == null || item.itemData == null)
            {
                continue;
            }

            string saveId = item.itemData.saveID;

            if (data.materialStash.ContainsKey(saveId) == false)
                data.materialStash[saveId] = 0;

            data.materialStash[saveId] += item.stackSize;
        }
    }

    public override void LoadData(GameData data)
    {
        GetInventoryItems().Clear();
        GetMaterialStash().Clear();

        foreach (var entry in data.storageItems)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            Item_DataSO itemToAdd = itemDatabase.GetItemData(saveId);

            if (itemToAdd == null)
            {
                Debug.LogWarning($"Item with SaveID {saveId} not found in database. Skipping load for this item.");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemToAdd);
                AddItem(itemToLoad);
            }
        }

        foreach (var entry in data.materialStash)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            Item_DataSO itemToAdd = itemDatabase.GetItemData(saveId);

            if (itemToAdd == null)
            {
                Debug.LogWarning($"Item with SaveID {saveId} not found in database. Skipping load for this item.");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemToAdd);
                AddMaterialToStash(itemToLoad);
            }
        }
    }

}