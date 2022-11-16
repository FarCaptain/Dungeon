using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public Room RoomPrefab;
    public TilemapVisualizer tilemapVisualizer;
    [SerializeField] private GameObject character;

    private Vector2 CircleCenter = new Vector2(0.0f, 0.0f);
    public int Radius = 15;

    public int RoomAmount = 20;
    public int MeanRoomArea = 25;
    public int RoomAreaDevision = 12;

    public float PickedPropotion = 0.33f;

    public List<Room> RoomList = new List<Room>();
    private Dictionary<int, List<Edge>> Dis = new Dictionary<int, List<Edge>>();
    HashSet<Vector2Int> allTiles = new HashSet<Vector2Int>();

    private void Start()
    {
        //SpawnRooms();
    }

    public void SpawnRooms()
    {
        character.SetActive(false);
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
        yield return new WaitForSeconds(5.0f);
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

        System.Random randomizer = new System.Random();
        Vector2Int[] asArray = allTiles.ToArray();
        var tile = asArray[randomizer.Next(asArray.Length)];

        character.transform.position = (Vector3Int)tile;
        character.SetActive(true);
    }

    private void GenerateCorridors()
    {
        // generate corridors
        // uses manhattan distance
        Dis.Clear();
        for (int i = 0; i < RoomList.Count; i++)
            Dis.Add(i, new List<Edge>());
        for (int i = 0; i < RoomList.Count; i++)
        {
            for (int j = 0; j < RoomList.Count; j++)
            {
                if (i == j)
                    continue;
                float manh = Mathf.Abs(RoomList[i].center.x - RoomList[j].center.x) + Mathf.Abs(RoomList[i].center.y - RoomList[j].center.y);
                Dis[i].Add(new Edge(i, j, manh));
                Dis[j].Add(new Edge(j, i, manh));
            }
        }

        HashSet<Vector2Int> corriTiles = new HashSet<Vector2Int>();
        //Prim Algo find MST

        allTiles.Clear();
        HashSet<int> vis = new HashSet<int>();
        var proirQue = new List<Edge>();

        //starts from node0
        vis.Add(0);
        for (int i = 0; i < Dis[0].Count; i++)
            proirQue.Add(Dis[0][i]);

        while(proirQue.Count > 0)
        {
            proirQue.Sort(delegate (Edge a, Edge b) { return a.dis.CompareTo(b.dis); });
            Edge e = proirQue[0];
            proirQue.Remove(e);

            if (vis.Contains(e.to))
                continue;

            vis.Add(e.to);
            for (int i = 0; i < Dis[e.to].Count; i++)
                proirQue.Add(Dis[e.to][i]);

            float x1 = RoomList[e.from].center.x;
            float x2 = RoomList[e.to].center.x;

            float y1 = RoomList[e.from].center.y;
            float y2 = RoomList[e.to].center.y;

            float xl = Mathf.Min(x1, x2);
            float xr = Mathf.Max(x1, x2);
            float yl = Mathf.Min(y1, y2);
            float yr = Mathf.Max(y1, y2);

            for (float y = yl; y < yr; y += 1)
            {
                //corriTiles.Add(new Vector2Int((int)x1-1, (int)y));
                corriTiles.Add(new Vector2Int((int)x1, (int)y));
                corriTiles.Add(new Vector2Int((int)x1+1, (int)y));
            }
            for (float x = xl; x <= xr; x += 1)
            {
                corriTiles.Add(new Vector2Int((int)x, (int)y2-1));
                corriTiles.Add(new Vector2Int((int)x, (int)y2));
                //corriTiles.Add(new Vector2Int((int)x, (int)y2+1));
            }

        }

        //tilemapVisualizer.PaintFloorTiles(corriTiles);

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