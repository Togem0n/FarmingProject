using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public static class Settings
{
    public const string PersistentScene = "PersistentScene";

    public static int playerInitialInventoryCapacity = 9;
    public static int playerMaximumInventoryCapacity = 27;

    public const float secondsPerGameSecond = 0.012f;

    public const float gridCellSize = 1f;
    public const float gridCellDiagonalSize = 1.41f;
    public const int maxGridWidth = 99999;
    public const int maxGridHeight = 99999;
    public static float pixelSize = 0.0625f;

    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "WateringCan";
    public const string CollectingTool = "Basket";

    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;


    static Settings()
    {
        walkUp = Animator.StringToHash("walk_up");
        walkDown = Animator.StringToHash("walk_down");
        walkLeft = Animator.StringToHash("walk_left");
        walkRight = Animator.StringToHash("walk_right");
        eventAnimation = Animator.StringToHash("eventAnimation");
    }

}
