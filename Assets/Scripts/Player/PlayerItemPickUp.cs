using UnityEngine;

public class PlayerItemPickUp : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            //Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickedUp && InventoryManager.Instance.TryAddItem(item.ItemCode, item.ItemQuantity))
            {
                //collision.transform.position = Vector3.MoveTowards(collision.transform.position, transform.parent.transform.position, 3 * Time.deltaTime);

                InventoryManager.Instance.AddItem(item.ItemCode, item.ItemQuantity, collision.gameObject);
                
            }
        }
    }
}
