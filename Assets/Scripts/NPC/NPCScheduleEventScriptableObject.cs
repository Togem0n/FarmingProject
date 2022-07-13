using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCScheduleEventList", menuName = "ScriptableObjects/NPC/NPC Schedule Event List")]
public class NPCScheduleEventScriptableObject : ScriptableObject
{
    [SerializeField]
    public List<NPCScheduleEvent> npcScheduleEventList;
}
