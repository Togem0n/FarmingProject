using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertyManager : SingletonMonoBehaviour<GridPropertyManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap tilemap;
    private Tilemap dugTilemap;
    private Tilemap wateredTilemap;
    private bool isFirstTimeSceneLoaded = true;

    [SerializeField] private Tile dugTile;

    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private CropDetailsScriptableObjects cropDetailsList = null;
    [SerializeField] private GridPropertyScriptableObjects[] gridPropertyScriptableObjectsArray = null;

    private string _iSaveablUniqueID;
    public string ISaveableUniqueID { get { return _iSaveablUniqueID; } set { _iSaveablUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        InitialiseGridProperties();

    }

    private void ClearDisplayGroundDecorations()
    {
        dugTilemap.ClearAllTiles();
        wateredTilemap.ClearAllTiles();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        Crop[] crops = FindObjectsOfType<Crop>();

        foreach(Crop crop in crops)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
    }
    
    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.daysSinceDug > -1)
        {
            SetTileToDug(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        }
    }

    // call when save scene
    private void DisplayGridPropertyDetails()
    {
        foreach(KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);

            // DisplayWaterGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if(gridPropertyDetails.seedItemCode > -1)
        {
            // get crop
            //Debug.Log(gridPropertyDetails.seedItemCode);
            CropDetails cropDetails = cropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            GameObject cropPrefab;

            int growthStages = cropDetails.growthDays.Length;

            int currentGrowthStage = 0;
            //int daysCounter = cropDetails.totalGrowDays;
            for(int i = growthStages - 1; i >=0; i--)
            {
                if(gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                {
                    currentGrowthStage = i;
                    break;
                }

                //daysCounter = daysCounter - cropDetails.growthDays[i];
            }

            cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

            Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

            //Debug.Log("current growth stage" + currentGrowthStage);

            Vector3 worldPosition = dugTilemap.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

            worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y + Settings.gridCellSize / 2, worldPosition.z);

            GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
            cropInstance.transform.SetParent(cropParentTransform);
            cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        }
    }

    /// <summary>
    /// Store the value from gridProperties scriptabl object in GameObjectSave sceneData;
    /// </summary>
    private void InitialiseGridProperties()
    {
        // loop through all gridproperties in the array
        foreach(GridPropertyScriptableObjects gridPropertyScriptableObject in gridPropertyScriptableObjectsArray)
        {
            // create dictionary of grid property details
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // loop through gridproperty in the current map
            foreach(GridProperty gridProperty in gridPropertyScriptableObject.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);
                
                if(gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiaggable = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;

                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            SceneSave sceneSave = new SceneSave();

            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            if (gridPropertyScriptableObject.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeLoaded", true);

            GameObjectSave.sceneData.Add(gridPropertyScriptableObject.sceneName.ToString(), sceneSave);
        }
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        gridPropertyDictionary[key] = gridPropertyDetails;
    }

    /// <summary>
    /// return the gridProeprtyDetails at the gridlocation for the supplied dictioanry, or null if no properties exist at that location
    /// </summary>

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }

    public void SetTileToDug(int gridX, int gridY)
    {
        dugTilemap.SetTile(new Vector3Int(gridX, gridY, 0), dugTile);
    }

    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterLocal(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        Crop crop = null;

        for(int i = 0; i < collider2DArray.Length; i++)
        {
            // Get it using layer?
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
            {
                break;
            }

            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
            {
                break;
            }
        }
        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return cropDetailsList.GetCropDetails(seedItemCode);
    }

    private void AfterSceneLoaded()
    {
        cropParentTransform = GameObject.FindWithTag("CropsParentTransform").transform;
        grid = GameObject.FindObjectOfType<Grid>();
        dugTilemap = GameObject.FindWithTag("DugTilemap").GetComponent<Tilemap>();
        wateredTilemap = GameObject.FindWithTag("WateredTilemap").GetComponent<Tilemap>();
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();

        foreach(GridPropertyScriptableObjects gridPropertyScriptableObjects in gridPropertyScriptableObjectsArray)
        {
            if(GameObjectSave.sceneData.TryGetValue(gridPropertyScriptableObjects.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for(int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        if(gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        if(gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);
                    }
                }
            }
        }
        DisplayGridPropertyDetails();
    }

    #region ISaveable
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            if(sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }

            if (isFirstTimeSceneLoaded)
            {
                EventHandler.CallInstantiateCropPrefabsEvent();
            }

            if(gridPropertyDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();

                DisplayGridPropertyDetails();
            }

            if (isFirstTimeSceneLoaded)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
    #endregion


}
