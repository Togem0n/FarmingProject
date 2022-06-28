using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridDetails
{
    public int gridX;
    public int gridY;

    // Grid System
    public bool isDiaggable = false;
    public bool canDropItem = false;
    public bool canPlaceFurniture = false;
    public bool isPath = false;
    public bool isNPCObstacle = false;

    // Crop System
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemCode = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;

    public GridDetails(int gridX, int gridY)
    {
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
