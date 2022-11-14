using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public Room RoomPrefab;
    public TilemapVisualizer tilemapVisualizer;

    private Vector2 CircleCenter = new Vector2(0.0f, 0.0f);
    public int Radius = 15;

    public int RoomAmount = 20;
    public int MeanRoomArea = 25;
    public int RoomAreaDevision = 12;

    public float PickedPropotion = 0.33f;

    public List<Room> RoomList = new List<Room>();

    private void Start()
    {
        //SpawnRooms();
    }

    public void SpawnRooms()
    {
        tilemapVisualizer.Clear();
        if (RoomList.Count > 0)
        {
            foreach (var item in RoomList)
            {
                Destroy(item.gameObject);
            }
            RoomList.Clear();
        }

        for (int i = 0; i < RoomAmount; i++)
        {
            Vector2 center = GetRandomPointInCircle();
            int area = Mathf.RoundToInt((float)GetT(MeanRoomArea, RoomAreaDevision));
            float ratio = UnityEngine.Random.Range(1f, Mathf.Sqrt(area)-0.1f);
            int w = Mathf.RoundToInt(Mathf.Sqrt(area * 1.0f / ratio));
            if(w == 0)
            {
                --i;
                continue;
            }
            int h = area / w;

            if(Random.Range(0, 10) >= 4 && (h > w))
            {
                int temp = w;
                w = h;
                h = temp;
            }


            Debug.Log("This is" + area + " :" + "w" + w + " :" + "h" + h);

            Room room = Instantiate(RoomPrefab, transform);
            room.center = center;
            room.w = w;
            room.h = h;
            room.UpdateTransform();
            RoomList.Add(room);
        }

        foreach (Room room in RoomList)
        {
            room.EnableCollision();
        }

        StartCoroutine("SelectRooms");
    }

    IEnumerator SelectRooms()
    {
        yield return new WaitForSeconds(1.5f);
        RoomList = RoomList.OrderBy(o => o.area).ToList();

        int pickCnt = Mathf.RoundToInt(RoomList.Count * 1.0f * PickedPropotion);
        int leftCnt = RoomList.Count - pickCnt;
        for (int i = 0; i < leftCnt; i++)
        {
            Destroy(RoomList[i].gameObject);
        }
        RoomList.RemoveRange(0, leftCnt);
        yield return new WaitForSeconds(0.1f);
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        HashSet<Vector2Int> allTiles = new HashSet<Vector2Int>();

        for (int i = 0; i < RoomList.Count; i++)
        {
            allTiles.UnionWith(RoomList[i].GetRoomTiles());
            Destroy(RoomList[i].gameObject);
        }
        RoomList.Clear();

        tilemapVisualizer.PaintFloorTiles(allTiles);
        WallGenerator.CreateWalls(allTiles, tilemapVisualizer);
    }

    private Vector2 GetRandomPointInCircle()
    {
        Vector2 ret = Random.insideUnitCircle * Radius + CircleCenter;
        return ret;
    }

    public static double GetT(double mean, double stdDev)
    {
        System.Random rand = new System.Random();
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                     System.Math.Sin(2.0 * System.Math.PI * u2);
        double randNormal =
                     mean + stdDev * randStdNormal;
        return randNormal;
    }
}
