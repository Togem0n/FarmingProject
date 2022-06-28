using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInventoryContainer : MonoBehaviour
{
    public List<ItemDetails> shopInventoryList;

    [SerializeField] public ItemLibrary shopInventoryScriptableObject;
    [SerializeField] public GameObject shopInventorySlotPrefab;


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
        for(int i = 0; i < shopInventoryList.Count; i++)
        {
            GameObject slot = Instantiate(shopInventorySlotPrefab, transform);
            slot.name = "shopSlot" + i.ToString();
            slot.transform.GetChild(0).GetComponent<Image>().sprite = shopInventoryList[i].itemSprite;
        }
    }
}
