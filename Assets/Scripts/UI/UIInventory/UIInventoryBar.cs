using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    //[SerializeField] private Sprite blankSprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    int test = 0;
    public GameObject inventoryBarDraggedItem;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }
    public UIInventorySlot[] InventorySlots { get => inventorySlots; set => inventorySlots = value; }

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
          
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        SetSelectedItemFromUserInput();

        SwitchInventoryBarPosition();
    }

    public void ClearHighLightOnInventorySlots()
    {
        if(InventorySlots.Length > 0)
        {
            for(int i = 0; i < InventorySlots.Length; i++)
            {
                if (InventorySlots[i].isSelected)
                {
                    InventorySlots[i].isSelected = false;
                    InventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    InventoryManager.Instance.ClearSelectedInventoryItem();
                }
            }
        }
    }

    public void SetHighlightedInventorySlots()
    {
        if(InventorySlots.Length > 0)
        {
            for(int i = 0; i < InventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if(InventorySlots.Length > 0 && InventorySlots[itemPosition].itemDetails != null)
        {
            if (InventorySlots[itemPosition].isSelected)
            {
                InventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                InventoryManager.Instance.SetSelectedInventoryItem(InventorySlots[itemPosition].itemDetails.itemCode, itemPosition);
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

    public void DestoryCurrentlyDraggedItem()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }

    public void SetSelectedItemFromUserInput()
    {

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]) && inventorySlots[i].itemDetails.itemCode != 0)
            {
                inventorySlots[i].SetSelectedItem();
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && GetCurrentCapacityOfInventorySlots() != 0)
        {
            int index = InventoryManager.Instance.SelectedItemIndex == -1? 0: InventoryManager.Instance.SelectedItemIndex;

            index += Mathf.FloorToInt(-Input.GetAxis("Mouse ScrollWheel") * 10);
            index %= GetCurrentCapacityOfInventorySlots();
            Debug.Log(index %= GetCurrentCapacityOfInventorySlots());
            index = index < 0 ? GetCurrentCapacityOfInventorySlots() + index : index;
            inventorySlots[index].SetSelectedItem();

        }

    }

    public void SetSelectedItem(int index)
    {
        inventorySlots[index].SetSelectedItem();
    }

    public void ClearSelectedItem()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }

    public int GetCurrentCapacityOfInventorySlots()
    {
        int num = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].itemDetails.itemCode != 0)
            {
                num++;
            }
        }
        return num;
    }
}
