using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    //public string name;
    public string itemName;
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public string itemLongDescription;
    public Sprite itemSprite;
    public short itemUseGridRadius;
    public float itemUseRadius;
    public int itemPrice;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;

    public ItemDetails(string itemName, int itemCode)
    {
        this.itemName = itemName;
        this.itemCode = itemCode;
        this.itemDescription = this.itemName + this.itemCode.ToString();
    }
}
