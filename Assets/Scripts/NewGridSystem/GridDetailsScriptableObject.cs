using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridDetails", menuName = "ScriptableObjects/Grid Details")]
public class GridDetailsScriptableObject : ScriptableObject
{
    public SceneName sceneName;
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    [SerializeField]
    public List<GridDetails> GridDetailsList;
}
