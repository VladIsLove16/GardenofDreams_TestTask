using Assets.WUG.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

public enum InventoryChangeType
{
    Pickup,
    Drop
}
public class SaveData
{
    public List<ItemDetailsSaveInfo> itemDetailsSaveInfos = new();
}
/// <summary>
/// Generates and controls access to the Item Database and Inventory Data
/// </summary>
public class InventoryController : MonoBehaviour
{
    [SerializeField]
    List<ItemDetails> ItemDetailsList;
    private Dictionary<string, ItemDetails> m_ItemDatabase = new Dictionary<string, ItemDetails>();
    private Dictionary<ItemDetails, int> m_PlayerInventory = new Dictionary<ItemDetails, int>();
    public event Action<InventoryChangeData> OnInventoryChanged = delegate { };
    public int MaxSlotsCount = 24;
    [SerializeField]
    public bool OpenOnStart;
    private void Awake()
    {
        PopulateDataBase();
        bool InventoryLoadResult = LoadInventory();
        if (!InventoryLoadResult)
        {
            AddDefaultItemsToInventory();
        }
    }

    /// <summary>
    /// Populate the database
    /// </summary>
    

    /// <summary>
    /// Retrieve item details based on the GUID
    /// </summary>
    /// <param name="guid">ID to look up</param>
    /// <returns>Item details</returns>
    public  ItemDetails GetItemByGuid(string guid)
    {
        if (m_ItemDatabase.ContainsKey(guid))
        {
            return m_ItemDatabase[guid];
        }

        return null;
    }
    public int GetItemCount(string guid)
    {
        ItemDetails itemDetails = GetItemByGuid(guid);
        if (m_PlayerInventory.Keys.Contains(itemDetails))
        {
            return m_PlayerInventory[itemDetails];
        }
        return 0;
    }
    public Dictionary<ItemDetails,int> GetAllItemDetails()
    {
        return m_PlayerInventory;
    }
    public Dictionary<string,int> GetAllItemDetailsByGuid()
    {
        var Items = m_PlayerInventory.ToDictionary(
        pair => pair.Key.GUID,
        pair => pair.Value );
        return Items;
    }
    public void AddOneItem()
    {
        ItemDetails itemDetails = m_ItemDatabase.Values.First();
        AddItem(itemDetails);
    }
    public void AddItem(ItemDetails itemDetails)
    {
        IEnumerable<ItemDetails> enumerable = Enumerable.Repeat(itemDetails, 1);
        AddItems(enumerable);
    }
    public void AddItems(IEnumerable< ItemDetails> itemDetails)
    {
        Dictionary<ItemDetails, int> items = itemDetails.ToDictionary(item => item, item => 1);
        AddItems(items);
    }
    private void AddItems(Dictionary<ItemDetails,int> items)
    {
        foreach (var item in items) {
            if (m_PlayerInventory.ContainsKey(item.Key))
            {
                m_PlayerInventory[item.Key] += item.Value;
            }
            else
                m_PlayerInventory.Add(item.Key, item.Value);
        }

        Dictionary<string, int> transformedItems = items
        .ToDictionary(
        pair => pair.Key.GUID, 
        pair => pair.Value     
        );
        InventoryChangeData inventoryChangeData = new InventoryChangeData()
        {
            Items = transformedItems,
            ChangeType = InventoryChangeType.Pickup
        };
        OnInventoryChanged.Invoke(inventoryChangeData);
    }
    public bool RemoveItem(ItemDetails item)
    {
        if(m_PlayerInventory.ContainsKey(item))
        {
            m_PlayerInventory[item] -= 1;
            if (m_PlayerInventory[item] == 0)
                m_PlayerInventory.Remove(item);
            InventoryChangeData inventoryChangeData = new InventoryChangeData()
            {
                Items = new Dictionary<string, int>
                {
                    { item.GUID,1 }
                },
                ChangeType = InventoryChangeType.Drop,
            };
            OnInventoryChanged.Invoke(inventoryChangeData);
            return true;
        }
        return false;
    }
    internal bool HaveSpace(ItemDetails itemDetails)
    {
       if(HaveSpace())
            return true;
       else if(m_PlayerInventory.Keys.Contains(itemDetails))
            return true;
       else
            return false;
    }
    internal bool HaveSpace()
    {
        if(m_PlayerInventory.Count <= MaxSlotsCount)
        {
            return true;
        }
        else
        {

            return false;
        }
    }
    private void PopulateDataBase()
    {
        foreach (var itemDetails in ItemDetailsList)
        {
            m_ItemDatabase.Add(itemDetails.GUID, itemDetails);
        }
    }
    #region InventorySavings 
    /// <summary>
    /// Inventory defaults items add function
    /// </summary>
    private void AddDefaultItemsToInventory()
    {
        foreach (var itemDetails in ItemDetailsList)
        {
            m_PlayerInventory.Add(itemDetails, 1);
        }
    }
    private void OnApplicationQuit()
    {
        SaveInventory();
    }
    private void SaveInventory()
    {
        SaveData data = new SaveData();
        foreach (var item in m_PlayerInventory)
        {
            data.itemDetailsSaveInfos.Add(new ItemDetailsSaveInfo()
            {
                ItemGUID = item.Key.GUID,
                count = item.Value
            });
        }
        JsonSaveSystem.Save(data, "PlayerInventory");
    }
    private bool LoadInventory()
    {
        try
        {
            SaveData data = JsonSaveSystem.Load<SaveData>("PlayerInventory");
            foreach (var item in data.itemDetailsSaveInfos)
            {
                ItemDetails itemDetails = ItemDetailsList.FirstOrDefault(x => x.GUID == item.ItemGUID);
                m_PlayerInventory.Add(itemDetails, item.count);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("JsonSaveSystem.Load<SaveData> went wrong with ex: " + e);
            return false;
        }

    }
    #endregion Inventory 
}
