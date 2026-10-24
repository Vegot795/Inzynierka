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
    public bool isUnlocked = false;
    public Room roomOwner = null;
    public MapGenerator.Corridor corridorOwner = null;
    public int x;
    public int y;
    public CellType type = CellType.empty;
    public EnvironmentData environmentData;
    public bool isOccupied = false;
}

