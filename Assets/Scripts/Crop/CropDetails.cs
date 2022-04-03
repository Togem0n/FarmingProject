using System;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public string name;
    [ItemCodeDescription]
    public int seedItemCode;
    public int[] growthDays;
    //public int totalGrowDays;
    public GameObject[] growthPrefab;
    public Sprite[] growthSprite;
    public Season[] seasons;
    public Sprite harvestedSprite;

    [ItemCodeDescription]
    public int harvestedTransfromItemCode;
    public bool hideCropBeforeHarvestedAniamtion;
    public bool disableCropCollidersBeforeHarvestedAnimation;
    public bool isHarvestedAnimation;
    public bool spawnCropProducedAtPlayerPosition;
    //public HarvestActionEffect harvestActionEffect;

    [ItemCodeDescription]
    public int[] harvestToolItemCode;
    public int[] requiredHarvestActions;

    [ItemCodeDescription]
    public int[] cropProducedItemCode;
    public int[] cropProducedMinQuantity;
    public int[] cropProducedMaxQuantity;
    public int daysToRegrow;

    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        return RequiredHarvestActionsForTool(toolItemCode) != -1;
    }

    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for(int i= 0; i < harvestToolItemCode.Length; i++)
        {
            if(harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
