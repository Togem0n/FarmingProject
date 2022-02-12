using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/Item/ItemList")]
public class SO_ItemList : ScriptableObject
{
    [SerializeField] 
    public List<ItemDetails> itemDetails;


}
