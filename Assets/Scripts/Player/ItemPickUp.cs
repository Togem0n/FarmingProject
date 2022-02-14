using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if(item != null)
        {
            //Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickedUp)
            {
                Debug.Log(item.ItemQuantity);
                InventoryManager.Instance.AddItem(item.ItemCode, item.ItemQuantity, collision.gameObject);
            }
        }
    }
}
