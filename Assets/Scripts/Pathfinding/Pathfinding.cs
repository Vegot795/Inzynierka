using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public bool smoothPath = true;

    private GridAdapter gridAdapter;

    private void Awake()
    {
        gridAdapter = FindAnyObjectByType<GridAdapter>();
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
    {
        if (gridAdapter == null)
        {
            Debug.LogError("[Pathfinding] No active GridAdapter found in the scene.", this);
            return null;
        }

        Node startNode = gridAdapter.NodeCordInWorld(startWorldPos);
        Node targetNode = gridAdapter.NodeCordInWorld(targetWorldPos);
        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Start or target node is null.");
            return new List<Vector3>();
        }

        var openSet = new Queueueue<Node>();
        var closedSet = new HashSet<Node>();
        var openSetLookup = new HashSet<Node>();

        startNode.GCost = 0;
        startNode.HCost = GetDistance(startNode, targetNode);
        startNode.Parent = null;

        openSet.Enqueueueue(startNode);
        openSetLookup.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeueueue();

            if (closedSet.Contains(currentNode))
            {
                continue;
            }

            openSetLookup.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in gridAdapter.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int estimatedGCost = currentNode.GCost + GetDistance(currentNode, neighbor);

                if (estimatedGCost < neighbor.GCost || !openSetLookup.Contains(neighbor))
                {
                    neighbor.GCost = estimatedGCost;
                    neighbor.HCost = GetDistance(neighbor, targetNode);
                    neighbor.Parent = currentNode;

                    openSetLookup.Add(neighbor);
                    openSet.Enqueueueue(neighbor);
                }
            }
        }
        return null;
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        var nodePath = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            nodePath.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        nodePath.Add(startNode);
        nodePath.Reverse();

        if (smoothPath)
        {
            nodePath = SmoothPath(nodePath);
        }

        var path = new List<Vector3>();
        for (int i = 1; i < nodePath.Count; i++)
        {
            path.Add(nodePath[i].worldPosition);
        }
        return path;
    }

    private List<Node> SmoothPath(List<Node> nodePath)
    {
        var smoothed = new List<Node> { nodePath[0] };
        int current = 0;

        while (current < nodePath.Count - 1)
        {
            int farthest = current + 1;

            for (int i = nodePath.Count - 1; i > current + 1; i--)
            {
                if (gridAdapter.HasLineOfSight(nodePath[current], nodePath[i]))
                {
                    farthest = i;
                    break;
                }
            }

            smoothed.Add(nodePath[farthest]);
            current = farthest;
        }

        return smoothed;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public Node CellFromWorldPoint(Vector3 worldPoint)
    {
        if (gridAdapter == null)
        {
            return null;
        }
        return gridAdapter.NodeCordInWorld(worldPoint);
    }
}
