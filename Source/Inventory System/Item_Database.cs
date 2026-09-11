using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Item_Database : MonoBehaviour
{
    public static Item_Database Instance { get; private set; }

    [SerializeField] private ItemDrop_Manager itemDropManager;

    public List<Item_DataSO> itemList;

    public Item_DataSO GetItemData(string saveID)
    {
        return itemList.FirstOrDefault(item => item != null && item.saveID == saveID);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        itemDropManager.allItemDrops = itemList;
    }

#if UNITY_EDITOR
    [ContextMenu("Collect Item Data")]
    public void CollectItemData()
    {
        string[] guids = AssetDatabase.FindAssets("t:Item_DataSO");

        itemList = guids.Select(guid => AssetDatabase.LoadAssetAtPath<Item_DataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(item => item != null)
            .ToList();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
