using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;


    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        minute = minute - (minute % 10);

        string ampm = "";
        string sminute;

        if(hour > 12)
        {
            ampm = " pm";
        }
        else
        {
            ampm = " am";
        }

        if(hour >= 13)
        {
            hour -= 12;
        }

        if(minute < 10)
        {
            sminute = "0" + minute.ToString();
        }
        else
        {
            sminute = minute.ToString();
        }

        string time = hour.ToString() + " : " + sminute + ampm;

        timeText.SetText(time);
        dateText.SetText(dayOfWeek + ". " + day.ToString());
        seasonText.SetText(season.ToString());
        yearText.SetText("Year " + year);

    }
}
