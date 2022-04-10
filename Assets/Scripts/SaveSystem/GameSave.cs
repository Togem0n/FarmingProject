using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSave
{
    // string key = GUID gameObject ID;
    public Dictionary<string, GameObjectSave> gameObjectData;

    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
