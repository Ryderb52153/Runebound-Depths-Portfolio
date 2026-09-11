using System;

[Serializable]
public class Inventory_Item 
{
    public Item_Modifier[] itemModifiers { get; private set; }
    public Item_DataSO itemData;
    public int stackSize = 1;
    public Item_Effect_DataSO itemEffect;

    private string itemId;
    
    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
        this.itemModifiers = EquipmentData()?.GetItemModifiers();
        itemEffect = itemData.itemEffect;
        itemId = itemData.itemName + " - " + Guid.NewGuid();
    }

    public bool CanStackMore() => stackSize < itemData.maxStackSize;
    public void AddToStack(int amount) => stackSize += amount;
    public void RemoveFromStack(int amount) => stackSize -= amount;
    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe();


    public void AddModifier(Entity_Stats playerStats)
    {
        foreach (var mod in itemModifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemId);
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in itemModifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemId);
        }
    }

    private Equipment_DataSO EquipmentData()
    {
        if(itemData is Equipment_DataSO equipmentData)
            return equipmentData;

        return null;
    }
}