using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPropertyManager : SingletonMonoBehaviour<GridPropertyManager>
{
    public Grid grid;
    private Dictionary<string, GridPropertyDetail> gridPropertyDictionary;
    [SerializeField] private GridPropertyScriptableObjects gridPropertyScriptableObjects = null;

    private string _iSaveableUniquieID;
    public string ISaveableUniquieID { get => _iSaveableUniquieID; set => _iSaveableUniquieID = value; }

}
