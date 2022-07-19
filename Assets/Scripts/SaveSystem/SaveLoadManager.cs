using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    public GameSave gameSave;
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
    }

    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            gameSave = (GameSave)bf.Deserialize(file);

            for(int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }

            file.Close();
        }

        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        if(iSaveableObjectList == null)
        {
            Debug.Log("????????nmsl????????");
        }

        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        Debug.Log(Application.persistentDataPath + " / WildHopeCreek.dat");

        bf.Serialize(file, gameSave);

        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }

    public void StoreCurrentSceneData()
    {
        foreach(ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ReStoreCurrentSceneData()
    {
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void test()
    {
        Debug.Log("testttttttttttttttttttttttttttting");
    }
}
