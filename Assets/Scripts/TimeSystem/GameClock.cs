using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hourText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;
    [SerializeField] private TextMeshProUGUI moneyText = null;

    private void Start()
    {
        moneyText.text = Player.Instance.playerData.currentMoney.ToString();
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
        EventHandler.BuyItemEvent += UpdateMoneyValue;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
        EventHandler.BuyItemEvent -= UpdateMoneyValue;
    }

    private void UpdateMoneyValue()
    {
        moneyText.text = "Money: " + Player.Instance.playerData.currentMoney.ToString();
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

        string time = hour.ToString() + ":" + sminute + ampm;

        hourText.text = time;
        dateText.text = dayOfWeek + ". " + day.ToString();
        //dateText.text = dayOfWeek.ToLower();
        seasonText.text = season.ToString().ToLower();
        yearText.text = "Year " + year;

    }
}
