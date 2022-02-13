using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDictionary; // using itemCode to look up itemDetail

    [SerializeField] private ItemLibrary itemLibrary; // ItemLibrary

    private List<InventoryItem> inventoryList = new List<InventoryItem>(); // player's inventory list
    public List<InventoryItem> InventoryList { get => inventoryList; }

    private void Start()
    {
        InitItemDetailsDictionary();
    }

    /// <summary>
    /// Init the itemDatilsDitionary from the sriptable object item list
    /// </summary>
    private void InitItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemLibrary.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    public void AddItem(Item item, GameObject itemToDelete)
    {
        AddItem(item);

        Destroy(itemToDelete);
    }

    /// <summary>
    /// Add an item to the invenotry
    /// </summary>
    public void AddItem(Item item)
    {
        int itemCode = item.ItemCode;

        // Check if inventory already contains the item
        int itemIndex = CheckIfItemInInventory(itemCode);

        if (itemIndex != -1)
        {
            AddItemAtIndex(InventoryList, itemCode, itemIndex);
        }
        else
        {
            AddItemAtIndex(InventoryList, itemCode);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }


    /// <summary>
    /// Add item to the end of the inventory
    /// </summary>
    private void AddItemAtIndex(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);
        PrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add item to the inventory
    /// </summary>
    private void AddItemAtIndex(List<InventoryItem> inventoryList, int itemCode, int itemIndex)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[itemIndex].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[itemIndex] = inventoryItem;
        PrintInventoryList(inventoryList);
    }

    /// <summary>
    /// return index if item in inventory list
    /// return -1 if not
    /// </summary>
    private int CheckIfItemInInventory(int itemCode)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    private void PrintInventoryList(List<InventoryItem> inventoryList)
    {

        foreach (InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("Item Name: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemName + "   Item Quantity: " + inventoryItem.itemQuantity);
        }
        Debug.Log("************************************************************************");
    }
}
