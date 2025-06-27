using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public class Node //将地图块状化
    {
        public Vector2Int gridPos;
        public bool walkable;
        public Node parent;
        public float gCost, hCost;
        public float FCost => gCost + hCost;

        public Node(Vector2Int pos, bool walkable)
        {
            this.gridPos = pos;
            this.walkable = walkable;
        }
    }

    public class AStarPathfinder
    {
        private GridManager grid;

        public AStarPathfinder(GridManager grid)
        {
            this.grid = grid;
        }

        public List<Vector2> FindPath(Vector2 startWorld, Vector2 targetWorld)
        {
            Node start = grid.GetNodeFromWorld(startWorld);
            Node target = grid.GetNodeFromWorld(targetWorld);

            List<Node> openSet = new List<Node> { start };
            HashSet<Node> closedSet = new HashSet<Node>();

            start.gCost = 0;
            start.hCost = Vector2Int.Distance(start.gridPos, target.gridPos);

            while (openSet.Count > 0)
            {
                Node current = openSet[0];
                foreach (Node n in openSet)
                {
                    if (n.FCost < current.FCost || (n.FCost == current.FCost && n.hCost < current.hCost))
                        current = n;
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current == target)
                {
                    return RetracePath(start, target);
                }

                foreach (Node neighbor in grid.GetNeighbours(current))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                    float newCost = current.gCost + Vector2Int.Distance(current.gridPos, neighbor.gridPos);
                    if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newCost;
                        neighbor.hCost = Vector2Int.Distance(neighbor.gridPos, target.gridPos);
                        neighbor.parent = current;

                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }

            return null; // 无路径
        }

        List<Vector2> RetracePath(Node start, Node end)
        {
            List<Vector2> path = new List<Vector2>();
            Node current = end;
            while (current != start)
            {
                path.Add(grid.WorldFromNode(current));
                current = current.parent;
            }
            path.Reverse();
            return path;
        }
    }

    public int width = 50, height = 50;
    public float cellSize = 1f;
    public LayerMask obstacleMask;
    private Node[,] grid;

    void Awake()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Node[width, height];
        Vector2 bottomLeft = (Vector2)transform.position - new Vector2(width, height) * cellSize * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = bottomLeft + new Vector2(x + 0.5f, y + 0.5f) * cellSize;
                bool walkable = !Physics2D.OverlapBox(worldPos, Vector2.one * cellSize * 0.9f, 0f, obstacleMask);
                grid[x, y] = new Node(new Vector2Int(x, y), walkable);
            }
        }
    }

    public Node GetNodeFromWorld(Vector2 worldPos)
    {
        Vector2 bottomLeft = (Vector2)transform.position - new Vector2(width, height) * cellSize * 0.5f;
        int x = Mathf.Clamp(Mathf.FloorToInt((worldPos - bottomLeft).x / cellSize), 0, width - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((worldPos - bottomLeft).y / cellSize), 0, height - 1);
        return grid[x, y];
    }

    public Vector2 WorldFromNode(Node node)
    {
        Vector2 bottomLeft = (Vector2)transform.position - new Vector2(width, height) * cellSize * 0.5f;
        return bottomLeft + (Vector2)(node.gridPos + Vector2.one * 0.5f) * cellSize;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        Vector2Int[] dirs = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in dirs)
        {
            Vector2Int pos = node.gridPos + dir;
            if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
                neighbours.Add(grid[pos.x, pos.y]);
        }

        return neighbours;
    }
}
