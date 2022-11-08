using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] Tilemap floorTilemap;

    [SerializeField] TileBase floorTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPos)
    {
        PaintFloorTiles(floorPos, floorTilemap, floorTile);
    }

    private void PaintFloorTiles(IEnumerable<Vector2Int> floorPos, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in floorPos)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
    }
}
