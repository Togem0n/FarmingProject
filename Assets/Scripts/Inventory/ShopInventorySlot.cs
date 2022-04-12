using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopInventorySlot : MonoBehaviour
{
    private ShopInventoryContainer container;
    private TextMeshProUGUI text;
    private int price;

    private void Awake()
    {      
        container = GetComponentInParent<ShopInventoryContainer>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        InitPrice();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void InitPrice()
    {
        string sid = transform.gameObject.name.ToString().Substring(8);
        int index = int.Parse(sid);
        price = container.shopInventoryList[index].itemPrice;
        text.text = container.shopInventoryList[index].itemPrice.ToString();
    }

    public void BuyItem()
    {
        string sid = transform.gameObject.name.ToString().Substring(8);
        int index = int.Parse(sid);
        int itemcode = container.shopInventoryList[index].itemCode;

        if(InventoryManager.Instance.CheckIfCanAddItem(itemcode, 1) && Player.Instance.playerData.currentMoney >= price)
        {
            Player.Instance.playerData.currentMoney -= price;
            InventoryManager.Instance.AddItem(itemcode, 1);
            EventHandler.CallBuyItemEvent();
            Debug.Log("buy one item");
        }
        else
        {
            Debug.Log("inventory is full or not enough money");
        }
    }
}
