using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
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


