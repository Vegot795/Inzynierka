using UnityEngine;
using System.Collections.Generic;

public class GridAdapter : MonoBehaviour
{
    public MapGenerator MapGenerator;

    private Node[,] nodes;
    private int width;
    private int height;

    private bool EnsureGraph()
    {
        if (nodes != null)
        {
            return true;
        }
        if (MapGenerator == null || MapGenerator.Grid == null)
        {
            return false;
        }
        BuildNodeGraph();
        return true;
    }

    void BuildNodeGraph()
    {
        GridCell[,] grid = MapGenerator.Grid;
        width = grid.GetLength(0);
        height = grid.GetLength(1);
        nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = grid[x, y];
                bool walkable = IsWalkable(cell);
                Vector3 worldPos = new Vector3(x, y, 0);
                nodes[x, y] = new Node(walkable, worldPos, x, y);
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

    public List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                {
                    continue;
                }

                int checkX = node.x + dx;
                int checkY = node.y + dy;

                if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height)
                {
                    continue;
                }

                if (dx != 0 && dy != 0)
                {
                    if (!nodes[checkX, node.y].walkable || !nodes[node.x, checkY].walkable)
                    {
                        continue;
                    }
                }

                neighbors.Add(nodes[checkX, checkY]);
            }
        }
        return neighbors;
    }

    public Node NodeCordInWorld(Vector3 worldPosition)
    {
        if (!EnsureGraph())
        {
            return null;
        }

        int x = Mathf.Clamp(Mathf.RoundToInt(worldPosition.x), 0, width - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(worldPosition.y), 0, height - 1);

        return nodes[x, y];
    }

    public bool HasLineOfSight(Node from, Node to)
    {
        int x = from.x;
        int y = from.y;
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);
        int stepX = to.x > from.x ? 1 : -1;
        int stepY = to.y > from.y ? 1 : -1;
        int error = dx - dy;

        while (x != to.x || y != to.y)
        {
            int doubledError = error * 2;
            bool moveX = doubledError > -dy;
            bool moveY = doubledError < dx;

            if (moveX && moveY)
            {
                if (!nodes[x + stepX, y].walkable || !nodes[x, y + stepY].walkable)
                {
                    return false;
                }

                error += dx - dy;
                x += stepX;
                y += stepY;
            }
            else if (moveX)
            {
                error -= dy;
                x += stepX;
            }
            else
            {
                error += dx;
                y += stepY;
            }

            if (!nodes[x, y].walkable)
            {
                return false;
            }
        }

        return true;
    }

    public Node NodeLocation(int x, int y)
    {
        if (!EnsureGraph())
        {
            return null;
        }
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return nodes[x, y];
    }

    public void RebuildGraph()
    {
        if (MapGenerator == null || MapGenerator.Grid == null)
        {
            return;
        }
        BuildNodeGraph();
    }
}
