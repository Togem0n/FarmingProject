using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blankSprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
        SwitchInventoryBarPosition();
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if(inventorySlots.Length > 0 && inventoryList.Count > 0)
            {
                for(int i = 0; i < inventorySlots.Length; i++)
                {
                    if(i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if(itemDetails != null)
                        {
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if(inventorySlots.Length > 0)
        {
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blankSprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;
            }
        }
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlyerViewportPosition();

        if(playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }else if(playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }
}
