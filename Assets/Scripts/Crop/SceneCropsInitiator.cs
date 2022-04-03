using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCropsInitiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private GridPropertyScriptableObjects currentSceneGrid;
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private GameObject treePrefab;

    private void Start()
    {

    }

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateSceneCrops;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateSceneCrops;
    }

    private void InstantiateSceneCrops()
    {
        Debug.Log(currentSceneGrid.originX);
        Debug.Log(currentSceneGrid.originY);
        Debug.Log(currentSceneGrid.gridWidth);
        Debug.Log(currentSceneGrid.gridHeight);
        Debug.Log(currentSceneGrid.originX + currentSceneGrid.gridWidth);
        Debug.Log(currentSceneGrid.originY + currentSceneGrid.gridHeight);

        grid = GameObject.FindObjectOfType<Grid>();

        for (int i = currentSceneGrid.originX; i < currentSceneGrid.originX + currentSceneGrid.gridWidth; i++)
        {
            for(int j = currentSceneGrid.originY; j < currentSceneGrid.originY + currentSceneGrid.gridHeight; j++)
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
                            gridPropertyDetails.seedItemCode = Random.Range(20014, 20016);
                            gridPropertyDetails.growthDays = 0;
                            GridPropertyManager.Instance.SetGridPropertyDetails(i, j, gridPropertyDetails);
                        }else if(dice < 75)
                        {
                            gridPropertyDetails.daysSinceDug = -1;
                            gridPropertyDetails.daysSinceWatered = -1;
                            List<int> tmp = new List<int>();
                            tmp.Add(20011);
                            tmp.Add(20012);
                            int num = Random.Range(0, tmp.Count);
                            gridPropertyDetails.seedItemCode = tmp[num];
                            gridPropertyDetails.growthDays = 6;
                            GridPropertyManager.Instance.SetGridPropertyDetails(i, j, gridPropertyDetails);
                        }
                    }
                }
            }
        }
    }
}
