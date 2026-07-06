using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public Grid gridMap;
    public Room roomScript;
    public Room startRoom;
    public GameObject gridCellPref;

    [SerializeField] private EnvironmentData[] roomEnvironment;

    private GridCell[,] grid;
    private readonly List<GridCell> emptyCells = new List<GridCell>();

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    public void Start()
    {
        GenerateGrid(mapWidth, mapHeight);

        if (startRoom != null && roomEnvironment != null)
        {
            roomScript.SetRoomEnvironment(startRoom, roomEnvironment[0]);
            Debug.Log($"Start room environment set to: {roomEnvironment[0]}");
        }
    }

    public void GenerateGrid(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;
        grid = new GridCell[width, height];
        emptyCells.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newCellObject = Instantiate(gridCellPref, new Vector3(x, y, 0), Quaternion.identity);
                newCellObject.transform.SetParent(transform);

                GridCell newCell = newCellObject.GetComponent<GridCell>();

                if (newCell == null)
                {
                    Debug.LogError("[MapGenerator] gridCellPref does not contain GridCell component.");
                    continue;
                }

                newCell.x = x;
                newCell.y = y;
                newCell.type = GridCell.CellType.floor;
                newCell.environmentData = roomEnvironment[0];

                if (newCell.sr == null)
                {
                    newCell.sr = newCellObject.GetComponent<SpriteRenderer>();
                }

                if (newCell.sr != null)
                {
                    newCell.sr.sprite = roomEnvironment[0].Floor;
                }

                grid[x, y] = newCell;
                emptyCells.Add(newCell);
            }
        }

        Debug.Log($"Grid generated with dimensions: {width}x{height}. Cell count: {emptyCells.Count}");

        startRoom = roomScript.CreateNewRoom(emptyCells);

        Debug.Log($"startRoom created: {startRoom?.roomName}");
    }
}
