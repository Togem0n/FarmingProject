using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>, ISaveable
{
    #region Variables
    [SerializeField] private ItemLibrary itemLibrary;
    [SerializeField] private UIInventoryBar inventoryBar;
    [SerializeField] private PlayerData playerData;

    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    private int selectedItemCode = -1;
    private int selectedItemIndex = -1;
    private string iSaveableUniqueID;
    private GameObjectSave _gameObjectSave;
    private List<InventoryItem> inventoryList = new List<InventoryItem>();
    #endregion

    #region Get/Setter
    public int SelectedItemCode { get => selectedItemCode; set => selectedItemCode = value; }
    public int SelectedItemIndex { get => selectedItemIndex; set => selectedItemIndex = value; }
    public List<InventoryItem> InventoryList { get => inventoryList; }
    public string ISaveableUniqueID { get => iSaveableUniqueID; set => iSaveableUniqueID = value; }
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    #endregion

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        InitInventoryItemList();
        InitItemDetailsDictionary();
    }

    #region Init inventory list
    /// <summary>
    /// Initialize Inventory List with blank item with the size of current Inventory capacity
    /// </summary>
    public void InitInventoryItemList()
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = 0;
        inventoryItem.itemQuantity = 0;

        for(int i = 0; i < playerData.currentInventoryCapacity; i++)
        {
            inventoryList.Add(inventoryItem);
        }
        // PrintInventoryList(InventoryList);
    }

    /// <summary>
    /// Initialize the itemDatilsDitionary from the sriptable object item list
    /// </summary>
    private void InitItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemLibrary.itemDetailsLibrary)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }
    #endregion

    #region Add item
    /// <summary>
    /// called when player can pick up item and then destory it
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="itemQuantity"></param>
    /// <param name="itemToDelete"></param>
    public void AddItem(int itemCode, int itemQuantity, GameObject itemToDelete)
    {
        AddItem(itemCode, itemQuantity);
        
        Destroy(itemToDelete);
    }

    /// <summary>
    /// Add item into inventory list
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="itemQuantity"></param>
    public void AddItem(int itemCode, int itemQuantity)
    {
        int itemIndex = GetItemIndexInInventory(itemCode);

        if (itemIndex != -1)
        {
            AddItemAtIndex(itemCode, itemIndex, itemQuantity);
        }
        else
        {
            if (GetFirstBlankSlotIndex() != -1)
            {
                AddItemAtIndex(itemCode, GetFirstBlankSlotIndex(), itemQuantity);
            }
            else
            {
                return;
            }
        }
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
        return;
    }

    /// <summary>
    /// Try to add an item to the invenotry
    /// return true if successful
    /// return false if inventory is full
    /// </summary>
    public bool CheckIfCanAddItem(int itemCode, int itemQuantity)
    {
        int itemIndex = GetItemIndexInInventory(itemCode);

        if (itemIndex != -1)
        {
            return true;
        }
        else
        {
            return GetFirstBlankSlotIndex() != -1;
        }
    }

    /// <summary>
    /// Add item of a given quantity to the inventory if item already exist;
    /// </summary>
    public void AddItemAtIndex(int itemCode, int itemIndex, int itemQuantity)
    {
        if (CheckIfCanAddItem(itemCode, itemQuantity))
        {
            InventoryItem inventoryItem = new InventoryItem();

            inventoryItem.itemCode = itemCode;
            inventoryItem.itemQuantity = inventoryList[itemIndex].itemQuantity + itemQuantity;
            inventoryList[itemIndex] = inventoryItem;
            EventHandler.CallInventoryUpdatedEvent(InventoryList);
        }
    }
    #endregion

    #region remove item
    /// <summary>
    /// Remove number of item by 1
    /// if num of item is 0, replace by 0item
    /// </summary>
    public void RemoveItemByOneAtIndex(int itemIndex)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[itemIndex].itemQuantity - 1;
        int itemCode = InventoryList[itemIndex].itemCode;

        if(quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[itemIndex] = inventoryItem;
        }
        else
        {
            inventoryItem.itemQuantity = 0;
            inventoryItem.itemCode = 0;
            inventoryList[itemIndex] = inventoryItem;
        }
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }

    /// <summary>
    /// Remove number of selected item by 1
    /// if num of item is 0, replace by 0item
    /// </summary>
    public void RemoveSelectedItemByOne()
    {
        InventoryItem inventoryItem = new InventoryItem();

        int itemCode = InventoryList[SelectedItemIndex].itemCode;
        int quantity = inventoryList[SelectedItemIndex].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemCode = itemCode;
            inventoryItem.itemQuantity = quantity;
            inventoryList[SelectedItemIndex] = inventoryItem;
        }
        else
        {
            inventoryItem.itemCode = 0;
            inventoryItem.itemQuantity = 0;
            inventoryList[SelectedItemIndex] = inventoryItem;
            //ClearSelectedInventoryItem();
            inventoryBar.ClearHighLightOnInventorySlots();
        }
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }

    /// <summary>
    /// Remove all items at index and replace it by item0
    /// </summary>
    public void RemoveAllItemAtIndex(int itemIndex)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemQuantity = 0;
        inventoryItem.itemCode = 0;
        inventoryList[itemIndex] = inventoryItem;
        
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }
    #endregion

    #region swap item
    /// <summary>
    /// swap item
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void SwapItem(int first, int second)
    {
        InventoryItem firstItem = new InventoryItem();
        firstItem.itemCode = InventoryList[first].itemCode;
        firstItem.itemQuantity = InventoryList[first].itemQuantity;

        InventoryItem secondItem = new InventoryItem();
        secondItem.itemCode = InventoryList[second].itemCode;
        secondItem.itemQuantity = InventoryList[second].itemQuantity;

        inventoryList[first] = secondItem;
        inventoryList[second] = firstItem;
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }
    #endregion

    #region Other Functions
    /// <summary>
    /// return index if item in inventory list
    /// return -1 if not found
    /// </summary>
    private int GetItemIndexInInventory(int itemCode)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].itemCode == itemCode && itemCode != 0)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// find first blank slot's index
    /// return -1 if no blank slot
    /// </summary>
    /// <returns></returns>
    private int GetFirstBlankSlotIndex()
    {
        for(int i = 0; i < playerData.currentInventoryCapacity; i++)
        {
            if(inventoryList[i].itemCode == 0)
            {
                return i;
            }
        }
        return -1;
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        //ItemDetails itemDetails;
        //if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        //{
        //    return itemDetails;
        //}
        //else
        //{
        //    return null;
        //}
        return itemLibrary.GetItemDetails(itemCode);
    }

    public ItemDetails GetSelectedItemDetails()
    {
        //ItemDetails itemDetails;
        //if (itemDetailsDictionary.TryGetValue(selectedItemCode, out itemDetails))
        //{
        //    return itemDetails;
        //}
        //else
        //{
        //    return null;
        //}
        return itemLibrary.GetItemDetails(selectedItemCode);
    }

    public ItemDetails GetItemDetailsByIndex(int index)
    {
        int itemCode = GetItemCodeAtIndex(index);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    public int GetItemCodeAtIndex(int index)
    {
        return inventoryList[index].itemCode;
    }

    public int GetItemQuantitiesAtIndex(int index)
    {
        return inventoryList[index].itemQuantity;
    }

    public void SetSelectedInventoryItem(int itemCode, int index)
    {
        SelectedItemCode = itemCode;
        SelectedItemIndex = index;
    }

    public void ClearSelectedInventoryItem()
    {
        SelectedItemCode = -1;
        selectedItemIndex = -1;
    }

    public void PrintInventoryList(List<InventoryItem> inventoryList)
    {
        //Debug.Log(InventoryManager.Instance.InventoryList.Count);
        //foreach (InventoryItem inventoryItem in inventoryList)
        //{
        //    Debug.Log("Item Name: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemName + "   Item Quantity: " + inventoryItem.itemQuantity);
        //}
        //Debug.Log("************************************************************************");
    }
    #endregion

    #region Save inventory
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        SceneSave sceneSave = new SceneSave();

        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        sceneSave.listInventoryItem = InventoryList;

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if(sceneSave.listInventoryItem != null)
                {
                    inventoryList = sceneSave.listInventoryItem;

                    EventHandler.CallInventoryUpdatedEvent(inventoryList);
                }

                //inventoryBar.ClearHighLightOnInventorySlots();
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {

    }

    public void ISaveableRestoreScene(string sceneName)
    {

    }
    #endregion 
}
