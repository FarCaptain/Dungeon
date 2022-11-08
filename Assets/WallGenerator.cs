using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositios = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        foreach (var position in basicWallPositios)
        {
            tilemapVisualizer.PaintSingleBasicWall(position);
        }

        tilemapVisualizer.CompositeWallColliders();
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> cardinalDirectionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in cardinalDirectionsList)
            {
                var neighbourPostion = position + direction;
                if(floorPositions.Contains(neighbourPostion) == false)
                {
                    wallPositions.Add(neighbourPostion);
                }
            }
        }
        return wallPositions;
    }
}
