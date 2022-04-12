using UnityEngine;

public class PlayerItemPickUp : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickedUp && InventoryManager.Instance.CheckIfCanAddItem(item.ItemCode, item.ItemQuantity))
            {
                InventoryManager.Instance.AddItem(item.ItemCode, item.ItemQuantity, collision.gameObject);
            }
        }
    }
}
