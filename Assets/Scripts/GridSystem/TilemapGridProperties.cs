using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System;

[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private GridPropertyScriptableObjects gridPropertyScriptableObjects;
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if(gridPropertyScriptableObjects != null)
            {
                gridPropertyScriptableObjects.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridPropertyScriptableObjects != null)
            {
                EditorUtility.SetDirty(gridPropertyScriptableObjects);
            }
        }
    }

    private void UpdateGridProperties()
    {
        tilemap.CompressBounds();

        if (!Application.IsPlaying(gameObject))
        {
            if(gridPropertyScriptableObjects != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for(int x = startCell.x; x < endCell.x; x++)
                {
                    for(int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        if(tile != null)
                        {
                            gridPropertyScriptableObjects.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        //if (!Application.IsPlaying(gameObject))
        //{
        //    Debug.Log("DISABLE PROPERTY TILEMAPS");
        //}
    }
}
