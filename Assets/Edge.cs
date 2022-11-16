using System;
using System.Collections;
using UnityEngine;

public class Edge
{
    public int from;
    public int to;
    public float dis;
    public Edge(int _from, int _to, float _dis)
    {
        from = _from;
        to = _to;
        dis = _dis;
    }
    //public static bool operator <(Edge a, Edge b)
    //{
    //    return a.dis < b.dis;
    //}
    //public static bool operator >(Edge a, Edge b)
    //{
    //    return a.dis > b.dis;
    //}

    //EdgeComparer edgeComparer;
}
