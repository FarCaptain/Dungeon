using System;
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
    private Dictionary<Tuple<int, int>, float> Dis = new Dictionary<Tuple<int, int>, float>();
    HashSet<Vector2Int> allTiles = new HashSet<Vector2Int>();

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

            if(UnityEngine.Random.Range(0, 10) >= 4 && (h > w))
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

        GenerateCorridors();
        GenerateTiles();
    }

    private void GenerateCorridors()
    {
        // generate corridors
        // uses manhattan distance
        Dis.Clear();
        for (int i = 0; i < RoomList.Count; i++)
        {
            for (int j = 0; j < RoomList.Count; j++)
            {
                if (i == j)
                    continue;
                float manh = Mathf.Abs(RoomList[i].center.x - RoomList[j].center.x) + Mathf.Abs(RoomList[i].center.y - RoomList[j].center.y);
                Dis.Add(new Tuple<int, int>(i, j), manh);
            }
        }

        var sortedKeyValuePairs = Dis.OrderBy(x => x.Value).ToList();

        HashSet<Vector2Int> corriTiles = new HashSet<Vector2Int>();
        //Prim Algo find MST
        // starts from node 0

        allTiles.Clear();

        for (int i = 1; i < RoomList.Count; i++)
        {
            for (int j = 0; j < RoomList.Count; j++)
            {
                if (i == j) continue;
                //Dis[new Tuple<int, int>(0, i)]
                var key = new Tuple<int, int>(i, j);
                var pairKey = new KeyValuePair<Tuple<int, int>, float>(key, Dis[key]);
                if (sortedKeyValuePairs.Contains(pairKey))
                {
                    // first one found
                    float xl = RoomList[key.Item1].center.x;
                    float xr = RoomList[key.Item2].center.x;

                    if (xl > xr)
                    {
                        float tmp = xl;
                        xl = xr;
                        xr = tmp;
                    }

                    float yl = RoomList[key.Item1].center.y;
                    float yr = RoomList[key.Item2].center.y;

                    if (yl > yr)
                    {
                        float tmp = yl;
                        yl = yr;
                        yr = tmp;
                    }

                    
                    for (float y = yl; y < yr; y+=1)
                    {
                        corriTiles.Add(new Vector2Int((int)xl, (int)y));
                    }
                    for (float x = xl; x <= xr; x += 1)
                    {
                        corriTiles.Add(new Vector2Int((int)x, (int)yl));
                    }
                    break;
                }
            }
        }

        allTiles.UnionWith(corriTiles);
    }

    private void GenerateTiles()
    {
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
        Vector2 ret = UnityEngine.Random.insideUnitCircle * Radius + CircleCenter;
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
