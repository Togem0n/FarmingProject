using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCropsInitiator : MonoBehaviour
{
    private Grid grid;

    [SerializeField] private GridDetailsScriptableObject currentSceneGrid;
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private GameObject treePrefab;

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
        grid = GameObject.FindObjectOfType<Grid>();

        for (int i = currentSceneGrid.originX; i < currentSceneGrid.originX + currentSceneGrid.gridWidth; i++)
        {
            for(int j = currentSceneGrid.originY; j < currentSceneGrid.originY + currentSceneGrid.gridHeight; j++)
            {
                GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(i, j);

                if (gridDetails != null)
                {
                    if (gridDetails.isDiaggable)
                    {
                        int dice = Random.Range(0, 100);

                        if(dice < 80)
                        {
                            gridDetails.seedItemCode = -1;
                            continue;
                        }else if (dice < 90)
                        {
                            gridDetails.daysSinceDug = -1;
                            gridDetails.daysSinceWatered = -1;
                            gridDetails.seedItemCode = Random.Range(20014, 20016);
                            gridDetails.growthDays = 0;
                            GridDetailsManager.Instance.SetGridDetails(i, j, gridDetails);
                        }else if(dice < 100)
                        {
                            gridDetails.daysSinceDug = -1;
                            gridDetails.daysSinceWatered = -1;
                            List<int> tmp = new List<int>();
                            tmp.Add(20011);
                            tmp.Add(20012);
                            int num = Random.Range(0, tmp.Count);
                            gridDetails.seedItemCode = tmp[num];
                            gridDetails.growthDays = 6;
                            GridDetailsManager.Instance.SetGridDetails(i, j, gridDetails);
                        }
                    }
                }
            }
        }
    }
}
