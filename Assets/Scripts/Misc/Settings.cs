using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public const string PersistentScene = "PersistentScene";

    public static int playerInitialInventoryCapacity = 9;
    public static int playerMaximumInventoryCapacity = 27;

    public const float secondsPerGameSecond = 0.012f;

    public const float gridCellSize = 1f;

    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "WateringCan";
    public const string CollectingTool = "Basket";
}
