using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInventoryContainer : MonoBehaviour
{
    [SerializeField] public ItemLibrary shopInventoryScriptableObject;
    [SerializeField] GameObject shopInventorySlotPrefab;

    public List<ItemDetails> shopInventoryList;

    private void Awake()
    {
        shopInventoryList = shopInventoryScriptableObject.itemDetailsLibrary;
    }

    private void Start()
    {
        InitShopInventoryList();
    }

    private void InitShopInventoryList()
    {
        //Vector2 sizeOfContainer = new Vector2(transform.GetComponent<RectTransform>().rect.width, transform.GetComponent<RectTransform>().rect.height);
        //transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().rect.width, transform.GetComponent<RectTransform>().rect.height * shopInventoryList.Count);
        for(int i = 0; i < shopInventoryList.Count; i++)
        {
            GameObject slot = Instantiate(shopInventorySlotPrefab, transform);
            slot.name = "shopSlot" + i.ToString();

            slot.transform.GetChild(0).GetComponent<Image>().sprite = shopInventoryList[i].itemSprite;
        }
    }
}
