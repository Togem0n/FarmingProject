using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemtype;
    public ItemType SelectedItemtype { get => _selectedItemtype; set => _selectedItemtype = value; }
    

    private bool _cursorIsEnable = false;
    public bool CursorIsEnable { get => _cursorIsEnable; set => _cursorIsEnable = value; }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
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
            GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(GetGridPositionForCursor().x, GetGridPositionForCursor().y);
            if(gridPropertyDetails != null)
            {
                Debug.Log("*******************************************");
                Debug.Log("Coordination: ( " + GetGridPositionForCursor().x + ", " + GetGridPositionForCursor().y + ")");
                Debug.Log("can Drop Item: " + gridPropertyDetails.canDropItem);
                Debug.Log("is Diggable: " + gridPropertyDetails.isDiaggable);
                Debug.Log("days since dug: " + gridPropertyDetails.daysSinceDug);
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

        // // get grid property details at cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!gridPropertyDetails.canDropItem)
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.Commodity:

                    if (!gridPropertyDetails.canDropItem)
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.HoeingTool:
                    if (!gridPropertyDetails.isDiaggable)
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;
                case ItemType.WateringTool:
                    if (!gridPropertyDetails.isDiaggable)
                    {
                        SetCursorToInValid();
                        return;
                    }
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


}
