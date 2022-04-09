using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoTo = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();
    [SerializeField] private bool needClick = false;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private bool confineScreen = true;
    [SerializeField] public CinemachineVirtualCamera camera;


    private void OnTriggerEnter2D(Collider2D collision)
    {   
        Player player = collision.GetComponent<Player>();

        // camera.GetComponent<CinemachineConfiner>().m_ConfineScreenEdges = confineScreen;

        if (!needClick)
        {
            if (player != null)
            {
                //float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;
                //float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;
                float xPosition = scenePositionGoto.x;
                float yPosition = scenePositionGoto.y;
                float zPosition = 0f;

                SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoTo.ToString(), new Vector3(xPosition, yPosition, zPosition));
            }
        }
    }
}
