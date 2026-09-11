using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour, ISaveable
{
    [SerializeField] protected Item_Database itemDatabase;
    [SerializeField] private int maxInventorySize = 10;

    private List<Inventory_Item> itemList = new List<Inventory_Item>();

    protected Player player;
    public List<Inventory_Item> GetInventoryItems() => itemList;
    public event Action OnInventoryUpdated;

    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }

    public bool CanAddItem(Inventory_Item item)
    {   
        bool hasStackableSpace = FindStackableItem(item) != null;
        return hasStackableSpace || itemList.Count < maxInventorySize;
    }

    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item consumable = itemList.Find(item => item == itemToUse);

        if (consumable == null)
            return;

        if (consumable.itemEffect.CanBeUsed(player) == false)
            return;

        UseItem(consumable);
    }

    private void UseItem(Inventory_Item consumable)
    {
        consumable.itemEffect.Use(player);

        if (consumable.stackSize > 1)
            consumable.RemoveFromStack(1);
        else
            RemoveOneItem(consumable);

        TriggerUpdatedUI();
    }

    public Inventory_Item FindStackableItem(Inventory_Item itemToAdd)
    {
        return itemList.Find(item => item.itemData == itemToAdd.itemData && item.CanStackMore());
    }

    public void AddItem(Inventory_Item itemToAdd)
    {
        var existingStackableItem = FindStackableItem(itemToAdd);

        if (existingStackableItem != null)
            existingStackableItem.AddToStack(itemToAdd.stackSize);
        else
            itemList.Add(itemToAdd);

        TriggerUpdatedUI();
    }

    public void RemoveOneItem(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = itemList.Find(item => item == itemToRemove);

        if (itemInInventory.stackSize > 1)
            itemInInventory.RemoveFromStack(1);
        else
            itemList.Remove(itemToRemove);

        TriggerUpdatedUI();
    }

    public Inventory_Item FindItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item == itemToFind);
    }

    public Inventory_Item FindSameItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item.itemData == itemToFind.itemData);
    }

    public void TriggerUpdatedUI() => OnInventoryUpdated?.Invoke();

    public virtual void SaveData(ref GameData data)
    {
        
    }

    public virtual void LoadData(GameData data)
    {
        
    }
}