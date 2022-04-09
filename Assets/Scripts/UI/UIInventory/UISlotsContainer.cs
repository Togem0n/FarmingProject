using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlotsContainer : MonoBehaviour
{
    [SerializeField] private Sprite blankSprite = null;
    [SerializeField] private PlayerData data;
    [SerializeField] private Item item0;

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void Update()
    {

    }

    private void InventoryUpdated(List<InventoryItem> inventoryList)
    {
        ClearInventorySlots();

        if (transform.childCount > 0 && inventoryList.Count > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i < inventoryList.Count)
                {
                    int itemCode = inventoryList[i].itemCode;

                    ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                    if (itemDetails != null)
                    {
                        UIInventorySlot currentSlot = transform.GetChild(i).GetComponent<UIInventorySlot>();
                        currentSlot.inventorySlotImage.sprite = itemDetails.itemSprite;
                        currentSlot.textMeshProUGUI.text = inventoryList[i].itemQuantity == 0? "" : inventoryList[i].itemQuantity.ToString();
                        currentSlot.itemDetails = itemDetails;
                        currentSlot.itemQuantity = inventoryList[i].itemQuantity;
                    }

                }
                else
                {
                    break;
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                UIInventorySlot currentSlot = transform.GetChild(i).GetComponent<UIInventorySlot>();
                currentSlot.inventorySlotImage.sprite = blankSprite;
                currentSlot.textMeshProUGUI.text = "";
                currentSlot.itemDetails = null;
                currentSlot.itemQuantity = 0;
            }
        }
    }

}
