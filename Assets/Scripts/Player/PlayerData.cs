using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "PlayerData", menuName = "Data/PlayerData") ]
public class PlayerData : ScriptableObject
{
    [SerializeField] public float movementSpeed;
    [SerializeField] public int currentMoney;
    [SerializeField] public int maxReachMoney;
    [SerializeField] public int currentInventoryCapacity;
}
