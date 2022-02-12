using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode;
    public ItemType itemType;
    public string itemName;
    public string itemDescription;
    public string itemLongDescription;
    public Sprite itemSprite;
    public short itemUseGridRadius;
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}
