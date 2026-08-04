using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public SpriteRenderer sr;
    public enum CellType
    {
        empty,
        wallTop,
        wallBottom,
        wallLeft,
        wallRight,
        floor,
        cornerLeftTop,
        cornerRightTop,
        cornerLeftBottom,
        cornerRightBottom,
        corridorLeftTop,
        corridorLeftBottom,
        corridorRightTop,
        corridorRightBottom
    }

    public bool hasRoom = false;
    public Room roomOwner = null;
    public int x;
    public int y;
    public CellType type = CellType.empty;
    public EnvironmentData environmentData;

    public GridCell(int x, int y, CellType cellType, EnvironmentData environmentData)
    {
        this.x = x;
        this.y = y;
        this.type = cellType;
        this.environmentData = environmentData;

    }

}

public class NodeBase
{
    public NodeBase Connection { get; private set; }
    public float G {  get; private set; }
    public float H { get; private set; }
    public float F => G + H;

    public void SetConnection(NodeBase nodeBase) =>Connection = nodeBase;
    public void SetG(float g) => G = g;
    public void SetH(float h) => H = h;
}


