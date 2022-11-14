using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int w, h;
    public Vector2 center;
    private new Collider2D collider;
    public int area;

    public HashSet<Vector2Int> GetRoomTiles()
    {
        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
        float wlap = (float)w / 2f - 1f + 0.5f;
        float hlap = (float)h / 2f - 1f + 0.5f;

        Vector2 lu = new Vector2(center.x - wlap, center.y - hlap);

        for(int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Vector2Int pos = new Vector2Int((int)(lu.x + i), (int)(lu.y + j));
                tiles.Add(pos);
            }
        }

        return tiles;
    }

    public void EnableCollision()
    {
        if(collider == null)
            collider = GetComponent<Collider2D>();
        collider.enabled = true;
    }

    public void UpdateTransform()
    {
        transform.position = center;
        transform.localScale = new Vector3(w, h, 1.0f);
        area = w * h;
    }
}
