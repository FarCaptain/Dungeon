using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyKeepGenerator : MonoBehaviour
{
    public TilemapVisualizer TilemapVisualizer;

    List<Room> AllRooms = new List<Room>();

    private Vector2 ComputeSteering(Room curRoom)
    {
        Vector2 v = Vector2.zero;
        int neighborCnt = 0;

        foreach (Room room in AllRooms)
        {
            if (room == curRoom)
                continue;
            v.x += room.transform.position.x - curRoom.transform.position.x;
            v.y += room.transform.position.y - curRoom.transform.position.y;
            neighborCnt++;
        }

        v /= neighborCnt;
        v *= -1;
        v.Normalize();
        return v;
    }
}
