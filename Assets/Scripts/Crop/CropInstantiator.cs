using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;

    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    [SerializeField] private bool isStone; // stone item code 33 - 36

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefab;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefab;
    }

    private void Start()
    {
        if (isStone)
        {
            seedItemCode = Random.Range(20014, 20015);
        }
    }

    private void InstantiateCropPrefab()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        SetCropGridProperties(cropGridPosition);

        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (seedItemCode > 0)
        {

            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertyManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

            if(gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;

            GridPropertyManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);

        }
    }
}
