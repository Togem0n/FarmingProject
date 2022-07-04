using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    private AStar aStar;
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int endPosition;
    [SerializeField] private Tilemap tilemapToDisplayPathOn;
    [SerializeField] private Tile tileToUseToDisplayPath;
    [SerializeField] private bool displayStartAndEnd = false;
    [SerializeField] private bool displayPath = false;

    private Stack<NPCMovementStep> npcMovementSteps;

    private void Awake()
    {
        aStar = GetComponent<AStar>();

        npcMovementSteps = new Stack<NPCMovementStep>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(startPosition != null && endPosition != null && tilemapToDisplayPathOn != null  &&  tileToUseToDisplayPath != null)
        {
            if (displayStartAndEnd)
            {
                tilemapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), tileToUseToDisplayPath);

                tilemapToDisplayPathOn.SetTile(new Vector3Int(endPosition.x, endPosition.y, 0), tileToUseToDisplayPath);
            }
            else
            {
                tilemapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);

                tilemapToDisplayPathOn.SetTile(new Vector3Int(endPosition.x, endPosition.y, 0), null);
            }

            if (displayPath)
            {
                System.Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);

                aStar.BuildPath(sceneName, startPosition, endPosition, npcMovementSteps);

                foreach(NPCMovementStep nPCMovementStep in npcMovementSteps)
                {
                    tilemapToDisplayPathOn.SetTile(new Vector3Int(nPCMovementStep.gridCoordinate.x,
                        nPCMovementStep.gridCoordinate.y, 0), tileToUseToDisplayPath);
                }
            }
            else
            {
                if (npcMovementSteps.Count > 0)
                {
                    foreach (NPCMovementStep nPCMovementStep in npcMovementSteps)
                    {
                        tilemapToDisplayPathOn.SetTile(new Vector3Int(nPCMovementStep.gridCoordinate.x,
                            nPCMovementStep.gridCoordinate.y, 0), null);
                    }

                    npcMovementSteps.Clear();
                }
            }
        } 
    }
}
