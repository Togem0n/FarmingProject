using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>, ISaveable
{
    #region Variables
    private int selectedItemCode = -1;
    private int selectedItemIndex = -1;
    private string iSaveableUniqueID;
    private GameObjectSave _gameObjectSave;
    private List<InventoryItem> inventoryList = new List<InventoryItem>();
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    [SerializeField] private ItemLibrary itemLibrary;
    [SerializeField] private UIInventoryBar inventoryBar;
    [SerializeField] private PlayerData playerData;

    public int SelectedItemCode { get => selectedItemCode; set => selectedItemCode = value; }
    public int SelectedItemIndex { get => selectedItemIndex; set => selectedItemIndex = value; }
    public string ISaveableUniqueID { get => iSaveableUniqueID; set => iSaveableUniqueID = value; }
    public List<InventoryItem> InventoryList { get => inventoryList; }
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    #endregion

    #region Life Cycle

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

    #endregion

    #region Init Inventory List

    public void InitInventoryItemList()
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = 0;
        inventoryItem.itemQuantity = 0;

        for(int i = 0; i < playerData.currentInventoryCapacity; i++)
        {
            inventoryList.Add(inventoryItem);
        }
    }

    private void InitItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemLibrary.itemDetailsLibrary)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }
    #endregion

    #region Add Items To Inventory

    public void AddItem(int itemCode, int itemQuantity, GameObject itemToDelete)
    {
        AddItem(itemCode, itemQuantity);
        
        Destroy(itemToDelete);
    }

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

    #region Remove Items From Inventory

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
            inventoryBar.ClearHighLightOnInventorySlots();
        }
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }

    public void RemoveAllItemAtIndex(int itemIndex)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemQuantity = 0;
        inventoryItem.itemCode = 0;
        inventoryList[itemIndex] = inventoryItem;
        
        EventHandler.CallInventoryUpdatedEvent(InventoryList);
    }

    #endregion

    #region Swap Two Item In Inventory

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

    #region Get&Set Items

    public int GetItemIndexInInventory(int itemCode)
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

    public int GetFirstBlankSlotIndex()
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
        return itemLibrary.GetItemDetails(itemCode);
    }

    public ItemDetails GetSelectedItemDetails()
    {
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

    }

    #endregion

    #region ISaveable
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
