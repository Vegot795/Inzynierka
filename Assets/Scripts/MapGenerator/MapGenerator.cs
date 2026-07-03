using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public int mapWidth;
    public int mapHeight;
    public Grid gridMap;

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum Grid
    {
        empty,
        wall,
        floor
    }

    public void Start()
    {
        //GenerateGrid();
    }
    /*private void GenerateGrid()
    {
        gridMap = new Grid[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gridMap[x, y] = gridMap.empty;
            }
        }
    }

    Vector3Int TileCenter = new Vector3Int(gridMap.GetLenght(0) / 2,  gridMap.GEt);*/

}
