using UnityEngine;

public class AttractingItem : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if(item != null)
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickedUp && InventoryManager.Instance.CheckIfCanAddItem(item.ItemCode, item.ItemQuantity))
            {
                collision.transform.position = 
                    Vector3.MoveTowards(collision.transform.position, transform.parent.transform.position, 6 * Time.deltaTime);
            }
        }
    }
}
