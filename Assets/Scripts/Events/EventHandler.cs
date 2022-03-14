using System;
using System.Collections.Generic;

public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect, 
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);

public static class EventHandler
{
    public static event Action RemoveSelectedItemFroInventoryEvent;

    public static void CallRemoveSelectedIteFromInventoryEvent()
    {
        if(RemoveSelectedItemFroInventoryEvent != null)
        {
            RemoveSelectedItemFroInventoryEvent();
        }
    }

    // Inventory Updated Event
    public static event Action<List<InventoryItem>> InventoryUpdatedEvent;

    // Inventory Event call For Publishers
    public static void CallInventoryUpdatedEvent(List<InventoryItem> inventoryList)
    {
        if(InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryList);
        }
    }

    // Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event call For Publishers

    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        if(MovementEvent != null)
        {
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying,
                    toolEffect,
                    isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                    isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                    isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                    isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                    idleUp, idleDown, idleLeft, idleRight);
        }
    }

    // Time Event

    // Advance game minute
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(year, season, day, dayOfWeek, hour, minute, second);
        }
    }

    // Advance game hour
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(year, season, day, dayOfWeek, hour, minute, second);
        }
    }

    // Advance game day
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(year, season, day, dayOfWeek, hour, minute, second);
        }
    }

    // Advance game season
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(year, season, day, dayOfWeek, hour, minute, second);
        }
    }

    // Advance game year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(year, season, day, dayOfWeek, hour, minute, second);
        }
    }

    // Scene Load Event - in the order they happen
    // Before scene uoload Fade out evet

    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if(BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }



}