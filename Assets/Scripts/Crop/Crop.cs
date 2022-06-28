using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;

    [HideInInspector] public Vector2Int cropGridPosition;

    public void ProcessToolAction(ItemDetails equippedItemDetails)
    {
        GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(cropGridPosition.x, cropGridPosition.y);

        if(gridDetails == null)
        {
            return;
        }

        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridDetails.seedItemCode);
        if(seedItemDetails == null)
        {
            return;
        }

        CropDetails cropDetails = GridDetailsManager.Instance.GetCropDetails(seedItemDetails.itemCode);
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
            HarvestCrop(cropDetails, gridDetails);
        }
    }

    private void HarvestCrop(CropDetails cropDetails, GridDetails gridDetails)
    {
        gridDetails.seedItemCode = -1;
        gridDetails.growthDays = -1;
        gridDetails.daysSinceLastHarvest = -1;
        gridDetails.daysSinceWatered = -1;

        GridDetailsManager.Instance.SetGridDetails(gridDetails.gridX, gridDetails.gridY, gridDetails);

        SpawnHarvestItemsAndCreateHarvestedCrop(cropDetails, gridDetails);
    }

    private void SpawnHarvestItemsAndCreateHarvestedCrop(CropDetails cropDetails, GridDetails gridDetails)
    {
        SpawnHarvestedItems(cropDetails);

        if(cropDetails.harvestedTransfromItemCode > 0)
        {
            CreateHarvestedCrop(cropDetails, gridDetails);
        }

        Destroy(gameObject);
    }

    private void CreateHarvestedCrop(CropDetails cropDetails, GridDetails gridDetails)
    {
        gridDetails.seedItemCode = cropDetails.harvestedTransfromItemCode;
        gridDetails.growthDays = 0;
        gridDetails.daysSinceLastHarvest = -1;
        gridDetails.daysSinceWatered = -1;

        GridDetailsManager.Instance.SetGridDetails(gridDetails.gridX, gridDetails.gridY, gridDetails);
        GridDetailsManager.Instance.DisplayPlantedCrop(gridDetails);
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
                cropsToProduce = 
                    UnityEngine.Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            for(int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition = 
                    new Vector3
                    (transform.position.x + UnityEngine.Random.Range(-1f, 1f), 
                    transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0f);
                SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
            }

        }

    }
}
