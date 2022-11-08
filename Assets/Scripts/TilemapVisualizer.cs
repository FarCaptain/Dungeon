using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] Tilemap floorTilemap, wallTilemap;

    [SerializeField] TileBase floorTile, wallTop;

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

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }

    public void CompositeWallColliders()
    {
        wallTilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
    }
}
