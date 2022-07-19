using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : SingletonMonoBehaviour<TimeManager>, ISaveable
{
    private int year = 1;
    private Season season = Season.Spring;
    private int day = 1;
    private int hour = 6;
    private int minute = 0;
    private int second = 0;
    private string dayOfWeek = "Mon";

    private bool clockPaused = false;
    private float clockTick = 0f;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void AfterSceneLoadFadeIn()
    {
        clockPaused = false;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        clockPaused = true;
    }

    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(year, season, day, dayOfWeek, hour, minute, second);
    }

    private void Update()
    {
        if (!clockPaused)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        clockTick += Time.deltaTime;

        if(clockTick >= Settings.secondsPerGameSecond)
        {
            clockTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        second++;

        if(second > 59)
        {
            second = 0;
            minute++;

            if(minute > 59)
            {
                minute = 0;
                hour++;

                if (hour > 23)
                {
                    hour = 6;
                    day++;

                    if(day > 30)
                    {
                        day = 1;

                        int s = (int)season;
                        s++;
                        season = (Season)s;

                        if(s > 3)
                        {
                            s = 0;
                            season = (Season)s;
                            year++;

                            EventHandler.CallAdvanceGameYearEvent(year, season, day, dayOfWeek, hour, minute, second);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(year, season, day, dayOfWeek, hour, minute, second);
                    }

                    dayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(year, season, day, dayOfWeek, hour, minute, second);
                }

                EventHandler.CallAdvanceGameHourEvent(year, season, day, dayOfWeek, hour, minute, second);
            }
            EventHandler.CallAdvanceGameMinuteEvent(year, season, day, dayOfWeek, hour, minute, second);
        }
    }

    private string GetDayOfWeek()
    {
        int totalDays = ((int)season * 30) + day;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "mon";
            case 2:
                return "tue";
            case 3:
                return "wed";
            case 4:
                return "thu";
            case 5:
                return "fri";
            case 6:
                return "sat";
            case 0:
                return "sun";

            default:
                return "";
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(hour, minute, second);
        return gameTime;
    }

    public void TestAdvanceGameMinute()
    {
        for(int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }
    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        SceneSave sceneSave = new SceneSave();

        sceneSave.intDictionary = new Dictionary<string, int>();

        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.intDictionary.Add("Year", year);
        sceneSave.intDictionary.Add("day", day);
        sceneSave.intDictionary.Add("hour", hour);
        sceneSave.intDictionary.Add("minute", minute);
        sceneSave.intDictionary.Add("second", second);

        sceneSave.stringDictionary.Add("dayOfWeek", dayOfWeek);
        sceneSave.stringDictionary.Add("season", season.ToString());

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if(sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    if (sceneSave.intDictionary.TryGetValue("year", out int savedYear)) year = savedYear;
                    if (sceneSave.intDictionary.TryGetValue("day", out int savedDay)) day = savedDay;
                    if (sceneSave.intDictionary.TryGetValue("hour", out int savedHour)) hour = savedHour;
                    if (sceneSave.intDictionary.TryGetValue("minute", out int savedMinute)) minute = savedMinute;
                    if (sceneSave.intDictionary.TryGetValue("second", out int savedSecond)) second = savedSecond;
                    if (sceneSave.stringDictionary.TryGetValue("dayOfWeek", out string savedDayOfWeek)) dayOfWeek = savedDayOfWeek;
                }

                clockTick = 0f;

                EventHandler.CallAdvanceGameMinuteEvent(year, season, day, dayOfWeek, hour, minute, second);
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {

    }

    public void ISaveableRestoreScene(string sceneName)
    {

    }
}
