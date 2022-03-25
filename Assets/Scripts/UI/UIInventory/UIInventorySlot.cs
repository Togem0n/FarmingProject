using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Transform parentItem;
    public GameObject draggedItem;

    private GridCursor gridCursor;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private GameObject slotsContainer;

    [SerializeField] private UIInventoryBar inventoryBar = null;

    [HideInInspector] public ItemDetails itemDetails;

    [SerializeField] private GameObject itemPrefab = null;

    [HideInInspector] public int itemQuantity;

    [HideInInspector] public bool isSelected = false;

    private int id;
    private int draggedItemCode;
    private int draggedItemQuantity;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void Start()
    {
        SetSlotID();
        gridCursor = FindObjectOfType<GridCursor>();

        mainCamera = Camera.main;
    }

    private void ClearCursors()
    {
        gridCursor.DisableCursor();

        gridCursor.SelectedItemtype = ItemType.none;
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

        SetSelectedItem();
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
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                int endId = int.Parse(eventData.pointerCurrentRaycast.gameObject.name.ToString().Substring(15));
                InventoryManager.Instance.AddItemAtIndex(draggedItemCode, id, draggedItemQuantity);
                InventoryManager.Instance.SwapItem(id, endId);
                Destroy(draggedItem);

                inventoryBar.InventorySlots[endId].SetSelectedItem();

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

            // check if can drop
            Vector3Int gridPostion = GridPropertyManager.Instance.grid.WorldToCell(worldPosition);
            GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(gridPostion.x, gridPostion.y);

            //if (gridCursor.CursorPositionIsValid) 
            //{ 
            if (gridPropertyDetails != null && gridPropertyDetails.canDropItem)
            {
                GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = draggedItemCode;
                item.ItemQuantity = draggedItemQuantity;
                Debug.Log(item.ItemQuantity);
                inventoryBar.ClearHighLightOnInventorySlots();
            }
            else
            {
                InventoryManager.Instance.AddItemAtIndex(draggedItemCode, id, draggedItemQuantity);
            }

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

    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag("ItemParentTransform").transform;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                ClearSelectedItem();
            }
            else
            {
                if(itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
            //Debug.Log("select item: " + InventoryManager.Instance.SelectedItemCode);
        }
    }
    public void SetSelectedItem()
    {
        inventoryBar.ClearHighLightOnInventorySlots();

        isSelected = true;

        inventoryBar.SetHighlightedInventorySlots();

        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;

        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        gridCursor.SelectedItemtype = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedInventoryItem(itemDetails.itemCode, id);
        //Debug.Log("selected item's index is: " + id);
    }

    public void ClearSelectedItem()
    {
        ClearCursors();

        inventoryBar.ClearHighLightOnInventorySlots();

        isSelected = false;

        InventoryManager.Instance.ClearSelectedInventoryItem();
    }

}
