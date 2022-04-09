using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField] private PauseMenuInventroySlot[] inventoryManagementSlot = null;

    public GameObject inventroyMangementDraggedItemPrefab;

    [SerializeField] private Sprite transparent1616 = null;

    [SerializeField] PlayerData data;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;

        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryManager.Instance.InventoryList);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBoxGameobject();
    }

    public void DestroyInventoryTextBoxGameobject()
    {
        if(inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);
        }
    }

    public void DestroyCurrentlyDraggedItem()
    {
        for(int i = 0; i < InventoryManager.Instance.InventoryList.Count; i++)
        {
            if(inventoryManagementSlot[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(List<InventoryItem> invemtoryList)
    {
        InitialiseInventoryManagementSlots();

        for(int i = 0; i < InventoryManager.Instance.InventoryList.Count; i++)
        {
            inventoryManagementSlot[i].itemDetails = InventoryManager.Instance.GetItemDetails(invemtoryList[i].itemCode);
            inventoryManagementSlot[i].itemQuantity = invemtoryList[i].itemQuantity;

            if(inventoryManagementSlot[i].itemDetails != null)
            {
                inventoryManagementSlot[i].inventorySlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;

                inventoryManagementSlot[i].textMeshProUGUI.text = 
                    inventoryManagementSlot[i].itemQuantity == 0? 
                    "" : inventoryManagementSlot[i].itemQuantity.ToString();
            }

        }
    }

    private void InitialiseInventoryManagementSlots()
    {
        for(int i =0; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].inventorySlotImage.sprite = transparent1616;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }

        for (int i = data.currentInventoryCapacity; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(true);
        }

    }
}
