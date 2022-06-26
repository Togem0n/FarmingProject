public interface ISaveable
{
    string ISaveableUniqueID { get; set; }

    GameObjectSave GameObjectSave { get; set; }

    void ISaveableRegister();

    void ISaveableDeregister();

    GameObjectSave ISaveableSave(); // save game to file

    void ISaveableLoad(GameSave gameSave); // load game from file

    void ISaveableStoreScene(string sceneName);

    void ISaveableRestoreScene(string sceneName);
}