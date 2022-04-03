public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum ToolEffect
{
    none,
    watering
}

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

public enum ItemType
{
    Seed,
    Commodity,
    WateringTool,
    HoeingTool,
    ChoppingTool,
    BreakingTool,
    ReapingTool,
    CollectingTool,
    ReapableScenary,
    Furniture,
    none,
    count
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count,
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum GridType
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle
}

public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin,
    Scene4_Market,
    Scene5_Trod
}