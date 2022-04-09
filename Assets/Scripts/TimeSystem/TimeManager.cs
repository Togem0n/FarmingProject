using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
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


}
