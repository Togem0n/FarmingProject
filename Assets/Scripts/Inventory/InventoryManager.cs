using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    public List<InventoryItem>[] inventoryLists;

    [HideInInspector] public int[] inventoryListCapacityIntArray; // the index of the array is the inventory list(from the InventoryLocation enum), and the
                                                                  // value is the capacity of that inventory list

    [SerializeField] private SO_ItemList itemList = null;

    private void Start()
    {
        CrateInventoryLists();

        CreateIteDetailsDictionary();
    }

    private void CrateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for(int i=0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    /// <summary>
    /// Populates the itemDatilsDitionary fro the sriptable object ites list
    /// </summary>
    private void CreateIteDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach(ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    /// <summary>
    /// Add an item to the inventory list for the inventoryLocation and then destory the gameObjectToDelete
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// Add an item to the invenotry list for the inventoryLocation
    /// </summary>

    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if(itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// Add item to the end of the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add item to position in the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[itemPosition].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[itemPosition] = inventoryItem;

        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Find if an itemCode is already in the inventory. Return the position
    /// in the inventory list or -1 if the item is not in the inventory
    /// </summary>
    private int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for(int i = 0; i < inventoryList.Count; i++)
        {
            if(inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if(itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        
        foreach(InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("Item Name: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemName + "   Item Quantity: " + inventoryItem.itemQuantity);
        }
        Debug.Log("************************************************************************");
    }
}
