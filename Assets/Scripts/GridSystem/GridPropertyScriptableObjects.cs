using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// store the grid infos of each scenes like height, width, origin point and its gridProperty
/// </summary>
[CreateAssetMenu(fileName = "GridProperties", menuName = "ScriptableObjects/Grid Properties")]
public class GridPropertyScriptableObjects : ScriptableObject
{
    public SceneName sceneName;
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;
}
