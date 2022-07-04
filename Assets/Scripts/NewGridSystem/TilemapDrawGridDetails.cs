using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;

[ExecuteAlways]
public class TilemapDrawGridDetails : MonoBehaviour
{
    private Tilemap tilemap;

    [SerializeField] private bool isDiggable;
    [SerializeField] private bool canDropItem;
    [SerializeField] private bool isNPCObstacle;
    [SerializeField] private bool isPath;
    [SerializeField] private GridDetailsScriptableObject gridDetailsScriptableObject;

    private void OnEnable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridDetailsScriptableObject != null)
            {
                gridDetailsScriptableObject.GridDetailsList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridDetailsScriptableObject != null)
            {
                EditorUtility.SetDirty(gridDetailsScriptableObject);
            }
        }
    }

    private void UpdateGridProperties()
    {
        tilemap.CompressBounds();

        if (!Application.IsPlaying(gameObject))
        {
            if (gridDetailsScriptableObject != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            GridDetails gridDetails = new GridDetails(x, y);
                            gridDetails.isDiaggable = isDiggable;
                            gridDetails.canDropItem = canDropItem;
                            gridDetails.isNPCObstacle = isNPCObstacle;
                            gridDetails.isPath = isPath;
                            gridDetailsScriptableObject.GridDetailsList.Add(gridDetails);
                        }
                    }
                }
            }
        }
    }

}
