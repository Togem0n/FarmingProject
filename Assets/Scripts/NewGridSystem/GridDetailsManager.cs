using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridDetailsManager : SingletonMonoBehaviour<GridDetailsManager>, ISaveable
{

    private Tilemap dugTilemap;
    private Tilemap wateredTilemap;
    private Transform cropParentTransform;
    private bool isFirstTimeSceneLoaded = true;
    private Dictionary<string, GridDetails> gridDetailsDictionary;

    [HideInInspector] public Grid grid;

    [SerializeField] private Tile dugTile;
    [SerializeField] private Tile wateredTile;
    [SerializeField] private CropDetailsScriptableObjects cropDetailsList = null;
    [SerializeField] private GridDetailsScriptableObject[] gridDetailsScriptableObjectArray = null;

    private string _iSaveablUniqueID;
    public string ISaveableUniqueID { get { return _iSaveablUniqueID; } set { _iSaveablUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    #region LifeCycle

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

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        InitialiseGridDetails();
    }

    #endregion

    #region Events Related

    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        cropParentTransform = GameObject.FindWithTag("CropsParentTransform").transform;
        dugTilemap = GameObject.FindWithTag("DugTilemap").GetComponent<Tilemap>();
        wateredTilemap = GameObject.FindWithTag("WateredTilemap").GetComponent<Tilemap>();
    }

    private void AdvanceDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        ClearDisplayedGridDetails();
        Debug.Log("NMSL");
        foreach (GridDetailsScriptableObject gridDetailsScriptableObject in gridDetailsScriptableObjectArray)
        {
            if (GameObjectSave.sceneData.TryGetValue(gridDetailsScriptableObject.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridDetails> item = sceneSave.gridDetailsDictionary.ElementAt(i);

                        GridDetails gridDetails = item.Value;

                        if (gridDetails.growthDays > -1)
                        {
                            gridDetails.growthDays += 1;
                        }

                        if (gridDetails.daysSinceWatered > -1)
                        {
                            gridDetails.daysSinceWatered = -1;
                        }

                        SetGridDetails(gridDetails.gridX, gridDetails.gridY, gridDetails, sceneSave.gridDetailsDictionary);
                    }
                }
            }
        }
        DisplayGridDetails();
    }

    #endregion

    #region Crop Manager

    public void DisplayPlantedCrop(GridDetails gridDetails)
    {
        if (gridDetails.seedItemCode > -1)
        {
            CropDetails cropDetails = cropDetailsList.GetCropDetails(gridDetails.seedItemCode);

            GameObject cropPrefab;

            int growthStages = cropDetails.growthDays.Length;

            int currentGrowthStage = 0;

            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (gridDetails.growthDays >= cropDetails.growthDays[i])
                {
                    currentGrowthStage = i;
                    break;
                }
            }

            cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

            Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

            Vector3 worldPosition = dugTilemap.CellToWorld(new Vector3Int(gridDetails.gridX, gridDetails.gridY, 0));

            worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y + Settings.gridCellSize / 2, worldPosition.z);

            GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
            cropInstance.transform.SetParent(cropParentTransform);
            cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridDetails.gridX, gridDetails.gridY);
        }
    }

    public Crop GetCropObjectAtGridLocation(GridDetails gridDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterLocal(new Vector3Int(gridDetails.gridX, gridDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridDetails.gridX, gridDetails.gridY))
            {
                break;
            }

            crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridDetails.gridX, gridDetails.gridY))
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

    private void ClearAllCrops()
    {
        Crop[] crops = FindObjectsOfType<Crop>();

        foreach (Crop crop in crops)
        {
            Destroy(crop.gameObject);
        }
    }

    private void ClearAllDecorations()
    {
        dugTilemap.ClearAllTiles();
        wateredTilemap.ClearAllTiles();
    }

    private void ClearDisplayedGridDetails()
    {
        ClearAllCrops();
        ClearAllDecorations();
    }

    private void DisplayGridDetails()
    {
        foreach (KeyValuePair<string, GridDetails> item in gridDetailsDictionary)
        {
            GridDetails gridDetails = item.Value;

            DisplayDugGround(gridDetails);

            // DisplayWaterGround(gridPropertyDetails);

            DisplayPlantedCrop(gridDetails);
        }
    }

    public void DisplayDugGround(GridDetails gridDetails)
    {
        if (gridDetails.daysSinceDug > -1)
        {
            SetTileToDug(gridDetails.gridX, gridDetails.gridY);
        }
    }

    private void SetTileToDug(int gridX, int gridY)
    {
        dugTilemap.SetTile(new Vector3Int(gridX, gridY, 0), dugTile);
    }

    #endregion

    #region Grid Manager

    public GridDetails GetGridDetails(int gridX, int gridY)
    {
        return GetGridDetails(gridX, gridY, gridDetailsDictionary);
    }

    public GridDetails GetGridDetails(int gridX, int gridY, Dictionary<string, GridDetails> gridDetailsDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        GridDetails gridDetails;

        if (!gridDetailsDictionary.TryGetValue(key, out gridDetails))
        {
            return null;
        }
        else
        {
            return gridDetails;
        }
    }

    public void SetGridDetails(int gridX, int gridY, GridDetails gridDetails)
    {
        SetGridDetails(gridX, gridY, gridDetails, gridDetailsDictionary);
    }

    public void SetGridDetails(int gridX, int gridY, GridDetails gridDetails, Dictionary<string, GridDetails> gridDetailsDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        gridDetails.gridX = gridX;
        gridDetails.gridY = gridY;

        gridDetailsDictionary[key] = gridDetails;
    }
    
    private void InitialiseGridDetails()
    {
        foreach (GridDetailsScriptableObject gridDetailsScriptableObject in gridDetailsScriptableObjectArray)
        {
            Dictionary<string, GridDetails> gridDetailsDictionary = new Dictionary<string, GridDetails>();

            foreach (GridDetails gridDetails in gridDetailsScriptableObject.GridDetailsList)
            {
                int gridX = gridDetails.gridX;
                int gridY = gridDetails.gridY;

                // ResetGridCropDetails(gridDetails);

                if(GetGridDetails(gridX, gridY, gridDetailsDictionary) == null)
                {
                    SetGridDetails(gridX, gridY, gridDetails, gridDetailsDictionary);
                }
            }

            SceneSave sceneSave = new SceneSave();

            sceneSave.gridDetailsDictionary = gridDetailsDictionary;

            if (gridDetailsScriptableObject.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridDetailsDictionary = gridDetailsDictionary;
            }

            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeLoaded", true);

            GameObjectSave.sceneData.Add(gridDetailsScriptableObject.sceneName.ToString(), sceneSave);
        }
    }

    private void ResetGridCropDetails(GridDetails gridDetails)
    {
        // Crop System
        gridDetails.daysSinceDug = -1;
        gridDetails.daysSinceWatered = -1;
        gridDetails.seedItemCode = -1;
        gridDetails.growthDays = -1;
        gridDetails.daysSinceLastHarvest = -1;
    }

    #endregion

    #region Use Tool Manager

    public bool IsHoeable(int gridX, int gridY)
    {
        GridDetails gridDetails = GetGridDetails(gridX, gridY);

        return gridDetails != null && gridDetails.daysSinceDug == -1 && gridDetails.seedItemCode == -1;
    }

    public void HoeingGround(int gridX, int gridY)
    {
        GridDetails gridDetails = GetGridDetails(gridX, gridY);
        gridDetails.daysSinceDug = 0;

        SetGridDetails(gridX, gridY, gridDetails);
        DisplayDugGround(gridDetails);
    }

    #endregion

    public bool GetGridDimensions(SceneName sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        foreach(GridDetailsScriptableObject gridDetailsScriptableObject in gridDetailsScriptableObjectArray)
        {
            if(gridDetailsScriptableObject.sceneName == sceneName)
            {
                gridDimensions.x = gridDetailsScriptableObject.gridWidth;
                gridDimensions.y = gridDetailsScriptableObject.gridHeight;

                gridOrigin.x = gridDetailsScriptableObject.originX;
                gridOrigin.y = gridDetailsScriptableObject.originY;
            }

            return true;
        }

        return false;
    }

    #region ISaveable
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridDetailsDictionary != null)
            {
                gridDetailsDictionary = sceneSave.gridDetailsDictionary;
            }

            if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool storedIsFirstTimeSceneLoaded))
            {
                isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
            }

            if (isFirstTimeSceneLoaded)
            {
                EventHandler.CallInstantiateCropPrefabsEvent();
            }

            if (gridDetailsDictionary.Count > 0)
            {
                Debug.Log(gridDetailsDictionary.Count);
                ClearDisplayedGridDetails();
                DisplayGridDetails();
            }
            
            if (isFirstTimeSceneLoaded)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return GameObjectSave;
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();

        sceneSave.gridDetailsDictionary = gridDetailsDictionary;

        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    #endregion
}
