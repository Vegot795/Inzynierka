using UnityEngine;

public class GridAdapter : MonoBehaviour
{
    public MapGenerator MapGenerator;

    private Node[,] nodes;
    private int width;
    private int height;

    void Awake()
    {

    }

    void BuildNodeGraph()
    {
        GridCell[,] grid = MapGenerator.Grid;
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        nodes = new Node[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GridCell cell = grid[x, y];
                bool walkable = IsWalkable(cell);
                Vector3 worldPos = new Vector3(x, y, 0);
                nodes[x, y] = new Node(worldPos, walkable, x, y);
            }
        }
    }

    private bool IsWalkable(GridCell cell)
    {
        if (cell == null)
        {
            return false; 
        }
        return cell.type == GridCell.CellType.floor;
    }
}
