using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PauseMenuInventroySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject greyedOutImageGO;

    [SerializeField] private PauseMenuInventoryManagement inventoryManagement = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [SerializeField] private GameObject inventoryTooltipsBoxPrefab;
    [HideInInspector] public GameObject inventoryTooltipsBox;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    public GameObject draggedItem;
    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnDisable()
    {
        DestroyInventoryTooltipsBox();    
    }

    #region Drag&Drop Item
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemQuantity != 0)
        {
            draggedItem = Instantiate(inventoryManagement.inventroyMangementDraggedItemPrefab, inventoryManagement.transform);

            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
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
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            if(eventData.pointerCurrentRaycast.gameObject != null 
                && eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventroySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventroySlot>().slotNumber;

                InventoryManager.Instance.SwapItem(slotNumber, toSlotNumber);

                inventoryManagement.DestroyInventoryTextBoxGameobject();
            }
        }
    }
    #endregion

    #region Tooltips
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            inventoryTooltipsBox = Instantiate(inventoryTooltipsBoxPrefab, transform.position, Quaternion.identity);
            inventoryTooltipsBox.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTooltipBox inventoryTooltipBox = inventoryTooltipsBox.GetComponent<UIInventoryTooltipBox>();

            string itemTypeDescription = itemDetails.itemType.ToString();

            inventoryTooltipBox.SetToolTipsText(itemDetails.itemName, itemTypeDescription, "",
                itemDetails.itemLongDescription, "", "");

            inventoryTooltipsBox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            inventoryTooltipsBox.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            /*if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTooltipsBox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTooltipsBox.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTooltipsBox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTooltipsBox.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }*/
        }
    }

/*    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }*/

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTooltipsBox();
    }

    private void DestroyInventoryTooltipsBox()
    {
        if (inventoryTooltipsBox != null)
        {
            Destroy(inventoryTooltipsBox);
        }
    }
    #endregion

}
