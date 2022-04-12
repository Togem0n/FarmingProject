using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    //public string name;
    public string itemName;
    public string itemDescription;
    public string itemLongDescription;

    public int itemCode;
    public int itemPrice;
    public short itemUseGridRadius;
    public float itemUseRadius;

    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;

    public ItemType itemType;
    public Sprite itemSprite;

    public ItemDetails(string itemName, int itemCode)
    {
        this.itemName = itemName;
        this.itemCode = itemCode;
        this.itemDescription = this.itemName + this.itemCode.ToString();
    }
}
