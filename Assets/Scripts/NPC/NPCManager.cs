using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : SingletonMonoBehaviour<NPCManager>
{
    [HideInInspector]
    public NPC[] npcArray;

    private AStar aStar;

    private void Awake()
    {
        base.Awake();

        aStar = GetComponent<AStar>();

        npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus();
    }

    private void SetNPCsActiveStatus()
    {
        foreach(NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if(npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public bool BuildPath
        (SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if(aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
