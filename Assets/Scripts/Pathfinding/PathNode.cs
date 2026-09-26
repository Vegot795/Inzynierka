using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public int x;
    public int y;
    public Vector3 worldPosition;
    public bool walkable;

    public Vector3Int cellPosition;
    public int GCost;
    public int HCost;
    public Node Parent;

    public Node(bool walkable, Vector3 worldPosition, int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.x = x;
        this.y = y;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(other.HCost);
        }
        return compare;
    }
    public int FCost => GCost + HCost;
}
