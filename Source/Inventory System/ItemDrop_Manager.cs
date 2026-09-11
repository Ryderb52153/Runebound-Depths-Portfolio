using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ItemDrop_Manager : MonoBehaviour
{
    [SerializeField] private Object_ItemPU itemPUPrefab;
    
    private static Dictionary<ItemType, Dictionary<ItemRarity, List<Object_ItemPU>>> itemDropDict;

    public List<Item_DataSO> allItemDrops { get; set; }

    private void Awake()
    {
        itemDropDict = new Dictionary<ItemType, Dictionary<ItemRarity, List<Object_ItemPU>>>();
    }

    private void Start()
    {
        SetupDictionary();
    }

    [ContextMenu("Spawn each item At Player")]
    public void SpawnItemAtPlayer()
    {
        SpawnItemPU(ItemType.Armor, ItemRarity.Common, Player.Instance.transform.position);
    }

    private void SetupDictionary()
    {
        foreach (var item in allItemDrops)
        {
            Object_ItemPU itemPU = Instantiate(itemPUPrefab, transform.position, Quaternion.identity);
            itemPU.transform.parent = transform;
            itemPU.SetUpItem(item);
            itemPU.gameObject.SetActive(false);

            if (!itemDropDict.ContainsKey(itemPU.Type))
                itemDropDict[itemPU.Type] = new Dictionary<ItemRarity, List<Object_ItemPU>>();

            if (!itemDropDict[itemPU.Type].ContainsKey(itemPU.Rarity))
                itemDropDict[itemPU.Type][itemPU.Rarity] = new List<Object_ItemPU>();

            itemDropDict[itemPU.Type][itemPU.Rarity].Add(itemPU);
        }
    }

    public static bool ChanceToDrop()
    {
        int randomValue = Random.Range(0, 100);
        return randomValue < 100; // temp value, normally 45
    }

    public static ItemRarity GetRandomRarity()
    {
        int randomValue = Random.Range(0, 100);
        if (randomValue < 50) // 50% 
            return ItemRarity.Common;
        else if (randomValue < 75) // 25% 
            return ItemRarity.Uncommon;
        else if (randomValue < 90) // 15% 
            return ItemRarity.Rare;
        else if (randomValue < 98) // 8% 
            return ItemRarity.Epic;
        else if (randomValue < 100) // 2% 
            return ItemRarity.Legendary;

        return ItemRarity.Common;
    }

    public static ItemType GetRandomItemType()
    {
        var itemTypes = System.Enum.GetValues(typeof(ItemType));
        int randomIndex = Random.Range(0, itemTypes.Length);
        return (ItemType)itemTypes.GetValue(randomIndex);
    }

    public static void SpawnItemPU(ItemType itemType, ItemRarity itemRarity, Vector3 position)
    {
        if (!itemDropDict.ContainsKey(itemType)) return;
        if (!itemDropDict[itemType].ContainsKey(itemRarity)) return;

        var itemsOfTypeAndRarity = itemDropDict[itemType][itemRarity];

        if (itemsOfTypeAndRarity.Count == 0) return;

        var randomIndex = Random.Range(0, itemsOfTypeAndRarity.Count);
        var itemToDrop = itemsOfTypeAndRarity[randomIndex];

        Object_ItemPU spawnedObject = ObjectPoolManager.SpawnObject(itemToDrop, position, Quaternion.identity);
        spawnedObject.PlayDropAnimation(position);
        spawnedObject.SetUpItem(itemToDrop.Item_DataSO);
    }

    public static void SpawnRandomItemPU(Vector3 position)
    {
        ItemType randomType = GetRandomItemType();
        ItemRarity randomRarity = GetRandomRarity();
        SpawnItemPU(randomType, randomRarity, position);
    }
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}