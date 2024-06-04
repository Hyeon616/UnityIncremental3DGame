using System.Collections.Generic;
using UnityEngine;

public class JPSBPathfinding : MonoBehaviour
{
    public Transform seeker;
    public Transform target;
    public Grid grid;
    private Dictionary<(Vector3, Vector3), List<Node>> pathCache = new Dictionary<(Vector3, Vector3), List<Node>>();

    void Awake()
    {
        grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("Grid not found in the scene. Please ensure there is a Grid object in the scene.");
        }
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (pathCache.TryGetValue((startPos, targetPos), out List<Node> cachedPath))
        {
            grid.path = cachedPath;
            return;
        }

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        PriorityQueue<Node> openSet = new PriorityQueue<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Enqueue(startNode, 0);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.Dequeue();

            if (currentNode == targetNode)
            {
                List<Node> path = RetracePath(startNode, targetNode);
                pathCache[(startPos, targetPos)] = path;
                grid.path = path;
                return;
            }

            closedSet.Add(currentNode);

            foreach (Node neighbour in IdentifySuccessors(currentNode, targetNode, closedSet))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Enqueue(neighbour, neighbour.fCost);
                }
            }
        }
    }

    List<Node> IdentifySuccessors(Node node, Node target, HashSet<Node> closedSet)
    {
        List<Node> successors = new List<Node>();
        foreach (Node direction in GetNeighbours(node))
        {
            Node jumpNode = Jump(direction, node, target);
            if (jumpNode != null && !closedSet.Contains(jumpNode))
            {
                successors.Add(jumpNode);
            }
        }
        return successors;
    }

    Node Jump(Node current, Node parent, Node target)
    {
        if (current == null || !current.walkable)
        {
            return null;
        }

        if (current == target)
        {
            return current;
        }

        int dx = current.gridX - parent.gridX;
        int dy = current.gridY - parent.gridY;

        if (dx != 0 && dy != 0)
        {
            if (grid.NodeFromWorldPoint(new Vector3(current.gridX - dx, 0, current.gridY)).walkable &&
                !grid.NodeFromWorldPoint(new Vector3(current.gridX - dx, 0, current.gridY - dy)).walkable ||
                grid.NodeFromWorldPoint(new Vector3(current.gridX, 0, current.gridY - dy)).walkable &&
                !grid.NodeFromWorldPoint(new Vector3(current.gridX - dx, 0, current.gridY - dy)).walkable)
            {
                return current;
            }
        }
        else
        {
            if (dx != 0)
            {
                if (grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY)).walkable &&
                    !grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY - 1)).walkable ||
                    grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY)).walkable &&
                    !grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY + 1)).walkable)
                {
                    return current;
                }
            }
            else if (dy != 0)
            {
                if (grid.NodeFromWorldPoint(new Vector3(current.gridX, 0, current.gridY + dy)).walkable &&
                    !grid.NodeFromWorldPoint(new Vector3(current.gridX - 1, 0, current.gridY + dy)).walkable ||
                    grid.NodeFromWorldPoint(new Vector3(current.gridX, 0, current.gridY + dy)).walkable &&
                    !grid.NodeFromWorldPoint(new Vector3(current.gridX + 1, 0, current.gridY + dy)).walkable)
                {
                    return current;
                }
            }
        }

        if (dx != 0 && dy != 0)
        {
            if (Jump(grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY)), current, target) != null ||
                Jump(grid.NodeFromWorldPoint(new Vector3(current.gridX, 0, current.gridY + dy)), current, target) != null)
            {
                return current;
            }
        }

        if (grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY)).walkable ||
            grid.NodeFromWorldPoint(new Vector3(current.gridX, 0, current.gridY + dy)).walkable)
        {
            return Jump(grid.NodeFromWorldPoint(new Vector3(current.gridX + dx, 0, current.gridY + dy)), current, target);
        }
        else
        {
            return null;
        }
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return 10 * (dstX + dstY) + (14 - 2 * 10) * Mathf.Min(dstX, dstY);
    }

    List<Node> GetNeighbours(Node node)
    {
        return grid.GetNeighbours(node);
    }
}
