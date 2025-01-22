using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Windows;

public enum InventoryChangeType
{
    Pickup,
    Drop
}
public delegate void OnInventoryChangedDelegate(string[] itemGuid, InventoryChangeType change);

/// <summary>
/// Generates and controls access to the Item Database and Inventory Data
/// </summary>
public class InventoryController : MonoBehaviour
{
    [SerializeField]
    List<ItemDetails> ItemDetails;
    private static Dictionary<string, ItemDetails> m_ItemDatabase = new Dictionary<string, ItemDetails>();
    private List<ItemDetails> m_PlayerInventory = new List<ItemDetails>();
    public event OnInventoryChangedDelegate OnInventoryChanged = delegate { };
    public int MaxSlotsCount = 24;
    [SerializeField]
    public bool OpenOnStart;
    private void Awake()
    {
        foreach(var itemDetails in ItemDetails)
        {
            m_ItemDatabase.Add(itemDetails.GUID,itemDetails);
        }
    }
    private void Start()
    {
        //Add the ItemDatabase to the players inventory and let the UI know that some items have been picked up
        AddItems(m_ItemDatabase.Values);
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
    public IEnumerable<ItemDetails> GetAllItemDetails()
    {
        return m_PlayerInventory;
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
        m_PlayerInventory.AddRange(itemDetails);
        OnInventoryChanged.Invoke(itemDetails.Select(x => x.GUID).ToArray(), InventoryChangeType.Pickup);
    }

    internal bool HaveSpace(ItemDetails itemDetails)
    {
       if(HaveSpace())
            return true;
       else if(m_PlayerInventory.Contains(itemDetails))
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
}
