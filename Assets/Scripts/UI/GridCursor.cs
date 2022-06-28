using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Grid grid;
    private Canvas canvas;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private CropDetailsScriptableObjects cropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemtype;
    public ItemType SelectedItemtype { get => _selectedItemtype; set => _selectedItemtype = value; }
    
    private bool _cursorIsEnable = false;
    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }


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
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnable)
        {
            DisplayCursor();
        }

        if (Input.GetMouseButtonDown(1))
        {
            GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(GetGridPositionForCursor().x, GetGridPositionForCursor().y);

            if (gridDetails != null)
            {
                Debug.Log("*******************************************");
                Debug.Log("Coordination: ( " + GetGridPositionForCursor().x + ", " + GetGridPositionForCursor().y + ")");
                Debug.Log("can Drop Item: " + gridDetails.canDropItem);
                Debug.Log("is Diggable: " + gridDetails.isDiaggable);
            }
            else
            {
                Debug.Log("*******************************************");
                Debug.Log("No grid property details contain");
            }
        }
    }

    private Vector3Int DisplayCursor()
    {
        if(grid != null)
        {
            Vector3Int gridPosition = GetGridPositionForCursor();

            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            SetCursorValidity(gridPosition, playerGridPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius ||
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInValid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedItemDetails();

        if (itemDetails == null)
        {
            SetCursorToInValid();
            return;
        }

        GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!gridDetails.canDropItem)
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.Commodity:

                    if (!gridDetails.canDropItem)
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.HoeingTool:
                case ItemType.WateringTool:
                case ItemType.ReapingTool:
                case ItemType.BreakingTool:
                case ItemType.ChoppingTool:
                case ItemType.CollectingTool:
                    if(!IsCursorValidForTool(gridDetails, itemDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;

                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
        else
        {
            SetCursorToInValid();
            return;
        }

    }

    private bool IsCursorValidForTool(GridDetails gridDetails, ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.HoeingTool:
                if(gridDetails.isDiaggable == true && gridDetails.daysSinceDug == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ItemType.WateringTool:
                if(gridDetails.daysSinceDug > -1 && gridDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ItemType.ChoppingTool:
            case ItemType.BreakingTool:
            case ItemType.CollectingTool:
                if (gridDetails.seedItemCode != -1)
                {
                    CropDetails cropDetails = cropDetailsList.GetCropDetails(gridDetails.seedItemCode);

                    if(cropDetails != null)
                    {
                        if (gridDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            default:
                return false;
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInValid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnable = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);

        CursorIsEnable = true;
    }

    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }
}
