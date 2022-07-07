using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int defaultMovementPenalty = 0;

    private GridNode gridNode;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closeNodeList;

    private bool pathFound = false;

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        pathFound = false; 

        if (PopulateGridNodesFromGridDetailsDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                Debug.Log("found path");
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
        }
        return false;
    }

    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;
        Debug.Log(targetNode.gridPosition);
        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            npcMovementStepStack.Push(npcMovementStep);

            nextNode = nextNode.parentNode;
        }
    }

    private bool FindShortestPath()
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            closeNodeList.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            EvaluateCurrentNodeNeighbours(currentNode);

        }

        return pathFound;
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                if (validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    if (observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }


                    string coordx = (currentNodeGridPosition.x + i).ToString();
                    string coordy = (currentNodeGridPosition.y + j).ToString();

               /*     Debug.Log("coordination x: " + coordx + " y: " + coordy + " gcost: " + 
                        currentNode.gCost + "distance cost = node a:" + 
                        currentNode.gridPosition.x + ", " + currentNode.gridPosition.y + 
                        "node b: " + validNeighbourNode.gridPosition.x + ", " + validNeighbourNode.gridPosition.y + "cost = " +
                        GetDistance(currentNode, validNeighbourNode));*/


                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        // 12 88 11 87
        int disX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int disY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (disX > disY)
        {
            return (14 * disY + 10 * (disX - disY));
        }
        return (14 * disX + 10 * (disY - disX));
    }

    private Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition)
    {
        if(neighbourNodeXPosition >= gridWidth || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNode.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;   
        }
    }

    private bool PopulateGridNodesFromGridDetailsDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        SceneSave sceneSave;

        if (GridDetailsManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            if (sceneSave.gridDetailsDictionary != null)
            {
                if(GridDetailsManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    gridNode = new GridNode(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    Debug.Log("gridWidth: " + gridWidth);
                    Debug.Log("gridHeight: " + gridHeight);
                    Debug.Log("originX: " + originX);
                    Debug.Log("originY: " + originY);


                    openNodeList = new List<Node>();

                    closeNodeList = new HashSet<Node>();
                }
                else
                {
                    return false;
                }

                startNode = gridNode.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                targetNode = gridNode.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        GridDetails gridDetails = GridDetailsManager.Instance.GetGridDetails(x + gridOrigin.x, y + gridOrigin.y, 
                            sceneSave.gridDetailsDictionary);

                        if (gridDetails != null)
                        {
                            if (gridDetails.isNPCObstacle == true)
                            {
                                Node node = gridNode.GetGridNode(x, y);
                                node.isObstacle = true;
                            } else if (gridDetails.isPath == true)
                            {
                                Node node = gridNode.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            else
                            {
                                Node node = gridNode.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}
