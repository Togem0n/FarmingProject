using UnityEngine;

public class AttractingItem : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if(item != null)
        {
            //Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.canBePickedUp && InventoryManager.Instance.TryAddItem(item.ItemCode, item.ItemQuantity))
            {
                collision.transform.position = Vector3.MoveTowards(collision.transform.position, transform.parent.transform.position, 6 * Time.deltaTime);

                //if (collision.transform.position.x - transform.parent.transform.position.x < 0.2f 
                //    && collision.transform.position.y - transform.parent.transform.position.y < 0.2f)
                //{
                //    InventoryManager.Instance.AddItem(item.ItemCode, item.ItemQuantity, collision.gameObject);
                //}
            }
        }
    }
}
