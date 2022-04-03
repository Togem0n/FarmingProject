using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/Item/ItemList")]
public class ItemLibrary : ScriptableObject
{
    //[NamedArrayAttribute(new string[] { "Neutral", "Happy", "Sad" })]
    [SerializeField] 
    public List<ItemDetails> itemDetailsLibrary;

    public ItemDetails GetItemDetails(int itemCode)
    {
        return itemDetailsLibrary.Find(x => x.itemCode == itemCode);
    } 

}
