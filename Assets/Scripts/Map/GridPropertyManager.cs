using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertyManager : SingletonMonoBehaviour<GridPropertyManager>, ISaveable
{
    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
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
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void Start()
    {
        InitialiseGridProperties();
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

            // populate grid property dictionary -
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
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }
}
