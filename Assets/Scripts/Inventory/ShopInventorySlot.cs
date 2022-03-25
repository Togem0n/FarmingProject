using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventorySlot : MonoBehaviour
{
    private ShopInventoryContainer container;

    private void Start()
    {
        container = GetComponentInParent<ShopInventoryContainer>();
    }

    public void BuyItem()
    {
        string sid = transform.gameObject.name.ToString().Substring(8);
        int index = int.Parse(sid);
        int itemcode = container.shopInventoryList[index].itemCode;

        if(InventoryManager.Instance.TryAddItem(itemcode, 1))
        {
            InventoryManager.Instance.AddItem(itemcode, 1);
            Debug.Log("buy one item");

        }
        else
        {
            Debug.Log("inventory is full");
        }
    }
}
