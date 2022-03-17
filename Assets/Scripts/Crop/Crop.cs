using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [HideInInspector] public Vector2Int cropGridPosition;
    private int harvestActionCount = 0;

    public void ProcessToolAction(ItemDetails equippedItemDetails)
    {
        GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if(gridPropertyDetails == null)
        {
            return;
        }

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if(seedItemDetails == null)
        {
            return;
        }

        CropDetails cropDetails = GridPropertyManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if(cropDetails == null)
        {
            return;
        }

        harvestActionCount += 1;

        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if(requiredHarvestActions == -1)
        {
            return;
        }

        if(harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetails, gridPropertyDetails);
        }
    }

    private void HarvestCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertyManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);
        Destroy(gameObject);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {

        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
               cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = UnityEngine.Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            if (InventoryManager.Instance.TryAddItem(cropDetails.cropProducedItemCode[i], cropsToProduce))
            {

            }
            else
            {
                Debug.Log("Inventory is full");
                Vector3 spawnPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f), transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0f);
                Debug.Log("Produce item code:" + cropDetails.cropProducedItemCode[i]);
                SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
            }
        }

    }
}
