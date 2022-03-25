using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescription]
    [SerializeField]
    private int _itemCode;

    [SerializeField]
    private int _itemQuantity;

    [SerializeField]
    private ItemLibrary itemLibrary;

    private SpriteRenderer spriteRenderer;
    public int ItemCode { get => _itemCode; set => _itemCode = value; }
    public int ItemQuantity { get => _itemQuantity; set => _itemQuantity = value; }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(ItemQuantity == 0)
        {
            ItemQuantity = 1;
        }

        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCode)
    {
        ItemCode = itemCode;
        spriteRenderer.sprite = itemLibrary.itemDetails[itemCode].itemSprite;
    }
}
