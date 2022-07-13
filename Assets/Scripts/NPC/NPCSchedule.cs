using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private NPCScheduleEventScriptableObject npcScheduleEventScriptableObject = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
    private NPCPath npcPath;

    private void Awake()
    {
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach(NPCScheduleEvent npcScheduleEvent in npcScheduleEventScriptableObject.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        npcPath = GetComponent<NPCPath>();
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvancedMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvancedMinute;
    }

    private void GameTimeSystem_AdvancedMinute(int year, Season season, int day, string dayOfWeek, int hour, int minute, int second)
    {
        int time = (hour * 100) + minute;

        NPCScheduleEvent matchingNPCScheduleEvent = null;

        foreach(NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            if(npcScheduleEvent.Time == time)
            {
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != day) continue;

                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != season) continue;

                if (npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather) continue;


                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }else if(npcScheduleEvent.Time > time)
            {
                break;
            }
        }

        if(matchingNPCScheduleEvent != null)
        {
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
}
