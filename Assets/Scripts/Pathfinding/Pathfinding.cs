using NUnit.Framework;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private GridAdapter gridAdapter;

    private void Awake()
    {
        gridAdapter = GetComponent<GridAdapter>();

    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
    {
        Node startNode = gridAdapter.GetNodeFromWorldPosition(startWorldPos);
        Node targetNode = gridAdapter.GetNodeFromWorldPosition(targetWorldPos);
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
                bool inOpenSet = openSetLookup.Contains(neighbor);

                if(estimatedGCost < neighbor.GCost || !inOpenSet)
                {
                    neighbor.GCost = estimatedGCost;
                    neighbor.HCost = GetDistance(neighbor, targetNode);
                    neighbor.Parent = currentNode;
                    if (!inOpenSet)
                    {
                        openSet.Enqueueueue(neighbor);
                        openSetLookup.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        var path = new List<Vector3>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if (dstX > dstY)
}
