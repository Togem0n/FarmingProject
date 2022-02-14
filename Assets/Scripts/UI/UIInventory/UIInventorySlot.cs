using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Transform parentItem;
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private GameObject slotsContainer;

    [SerializeField] private UIInventoryBar inventoryBar = null;

    [HideInInspector] public ItemDetails itemDetails;

    [SerializeField] private GameObject itemPrefab = null;

    [HideInInspector] public int itemQuantity;

    private int id;
    private int draggedItemCode;
    private int draggedItemQuantity;


    private void Start()
    {
        SetSlotID(); 

        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag("ItemParentTransform").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedItemCode = InventoryManager.Instance.InventoryList[id].itemCode;
        draggedItemQuantity = InventoryManager.Instance.InventoryList[id].itemQuantity;

        if (draggedItemCode != 0)
        {
            Player.Instance.DisablePlayerInputAndResetMovement();

            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
            InventoryManager.Instance.RemoveItemAtIndex(id);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            Destroy(draggedItem);

            // if drag ends over inventory bar, swap
            if(eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                int endId = int.Parse(eventData.pointerCurrentRaycast.gameObject.name.ToString().Substring(15));
                InventoryManager.Instance.AddItemAtIndex(draggedItemCode, id, draggedItemQuantity);
                InventoryManager.Instance.SwapItem(id, endId);
            }
            else
            {
                DropSelectedItemAtMousePosition();
            }

            Player.Instance.EnablePlayerInput();
        }
    }

    private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = draggedItemCode;
            item.ItemQuantity = draggedItemQuantity;
            Debug.Log(item.ItemQuantity);
        }
    }

    private int SetSlotID()
    {
        string sid = transform.gameObject.name.ToString().Substring(15);
        id = int.Parse(sid);
        return id;
    }

    private int GetSlotID()
    {
        return id;
    }
}
