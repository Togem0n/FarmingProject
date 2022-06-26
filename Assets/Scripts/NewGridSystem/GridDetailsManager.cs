using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]
public class GridDetailsManager : SingletonMonoBehaviour<GridDetailsManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap dugTilemap;
    private Tilemap wateredTilemap;
    private bool isFirstTimeSceneLoaded = true;

    [HideInInspector]public Grid grid;
    private Dictionary<string, GridDetails> gridDetailsDictionary;
    [SerializeField] private GridDetailsScriptableObject[] gridDetailsScriptableObjectArray = null;

    // ISaveable
    private string _iSaveablUniqueID;
    public string ISaveableUniqueID { get { return _iSaveablUniqueID; } set { _iSaveablUniqueID = value; } }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    #region Initialization
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
        //EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        //EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        InitialiseGridDetails();
    }
    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        //cropParentTransform = GameObject.FindWithTag("CropsParentTransform").transform;
        //dugTilemap = GameObject.FindWithTag("DugTilemap").GetComponent<Tilemap>();
        //wateredTilemap = GameObject.FindWithTag("WateredTilemap").GetComponent<Tilemap>();
    }

    #endregion

    #region Grid Manager
    private void InitialiseGridDetails()
    {
        foreach (GridDetailsScriptableObject gridDetailsScriptableObject in gridDetailsScriptableObjectArray)
        {
            Dictionary<string, GridDetails> gridDetailsDictionary = new Dictionary<string, GridDetails>();

            foreach (GridDetails gridDetails in gridDetailsScriptableObject.GridDetailsList)
            {
                int gridX = gridDetails.gridX;
                int gridY = gridDetails.gridY;

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
    #endregion

    #region Get&Set GridDetails
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
    #endregion

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
            if (sceneSave.gridPropertyDetailsDictionary != null)
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
                //ClearDisplayGridPropertyDetails();

                //DisplayGridPropertyDetails();
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
