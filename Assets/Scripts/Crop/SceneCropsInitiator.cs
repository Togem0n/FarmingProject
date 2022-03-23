using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCropsInitiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private GridPropertyScriptableObjects currentSceneGrid;
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private GameObject treePrefab;

    private int width;
    private int height;

    private void Start()
    {
        width = currentSceneGrid.gridWidth;
        height = currentSceneGrid.gridHeight;
    }

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateSceneCrops;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateSceneCrops;
    }

    //grid = GameObject.FindObjectOfType<Grid>();

    //    Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

    //SetCropGridProperties(cropGridPosition);

    //Destroy(gameObject);
    private void InstantiateSceneCrops()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        for (int i = currentSceneGrid.originX; i < currentSceneGrid.originX + width; i++)
        {
            for(int j = currentSceneGrid.originY; j < currentSceneGrid.originY + height; j++)
            {
                GridPropertyDetails gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(i, j);

                if (gridPropertyDetails != null)
                {
                    if (gridPropertyDetails.isDiaggable)
                    {
                        int dice = Random.Range(0, 100);

                        if(dice < 50)
                        {
                            continue;
                        }else if (dice < 65)
                        {
                            gridPropertyDetails.daysSinceDug = -1;
                            gridPropertyDetails.daysSinceWatered = -1;
                            gridPropertyDetails.seedItemCode = Random.Range(33, 35);
                            gridPropertyDetails.growthDays = 0;
                            GridPropertyManager.Instance.SetGridPropertyDetails(i, j, gridPropertyDetails);
                        }else if(dice < 75)
                        {
                            gridPropertyDetails.daysSinceDug = -1;
                            gridPropertyDetails.daysSinceWatered = -1;
                            gridPropertyDetails.seedItemCode = 31;
                            gridPropertyDetails.growthDays = 6;
                            GridPropertyManager.Instance.SetGridPropertyDetails(i, j, gridPropertyDetails);
                        }
                    }
                }
            }
        }
    }
}
