using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonoBehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag("ItemParentTransform").transform;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);    
    }

    public void ISaveableStoreScene(string sceneName)
    {
        GameObjectSave.sceneData.Remove(sceneName);

        // Get all items in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        // Loop through
        foreach(Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            sceneItemList.Add(sceneItem);
        }

        // Create list scene itesm in scene save and add to it
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemList;

        // add scene save to gameobject
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if(GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.listSceneItem != null)
            {
                DestorySceneItems();

                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    private void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);

        Item item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach(SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }

    private void DestorySceneItems()
    {
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        for(int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }
}
