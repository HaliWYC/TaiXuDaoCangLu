using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        if (Application.isPlaying) return;
        currentTilemap = GetComponent<Tilemap>();
        if (mapData != null)
        {
            mapData.tileProperties.Clear();
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying) return;
        currentTilemap = GetComponent<Tilemap>();
        UpdateTileProperties();
#if UNITY_EDITOR
        if (mapData != null)
        {
            EditorUtility.SetDirty(mapData);
        }
#endif        
    }

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();
        if(Application.isPlaying) return;
        if (mapData == null) return;
        //绘制地图左下角的坐标
        var startPos = currentTilemap.cellBounds.min;
        //绘制地图右上角的坐标
        var endPos = currentTilemap.cellBounds.max;

        for (var x = startPos.x; x < endPos.x; x++)
        {
            for (var y = startPos.y; y < endPos.y; y++)
            {
                var tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile == null) continue;
                var newTile = new TileProperty
                {
                    tileCoordinates = new Vector2Int(x,y),
                    gridType = gridType,
                    boolTypeValue = true
                };
                mapData.tileProperties.Add(newTile);
            }
        }
    }
}