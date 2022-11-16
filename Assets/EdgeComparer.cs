using System.Collections.Generic;

public class EdgeComparer : IComparer<Edge>
{
    public int Compare(Edge x, Edge y)
    {
        if (x.dis > y.dis) return 1;
        if (x.dis < y.dis) return -1;
        return 0;
    }
}