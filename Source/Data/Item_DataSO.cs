using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Material item", fileName = "Material Data - ")]
public class Item_DataSO : ScriptableObject
{
    public string saveID { get; private set; } = System.Guid.NewGuid().ToString();
    
    public ItemRarity itemRarity;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public EquipmentSlot equipmentSlot;
    public int maxStackSize = 1;

    [Header("Item Effect")]
    public Item_Effect_DataSO itemEffect;
   
}