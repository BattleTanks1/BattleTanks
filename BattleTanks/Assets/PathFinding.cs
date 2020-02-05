using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public bool visited = false;
}


public class PathFinding : MonoBehaviour
{
    private Vector2Int[] m_directions2D = new Vector2Int[4]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    List<Vector2Int> m_adjacentPositions = new List<Vector2Int>();

    private GraphNode[,] m_graph;

    private static PathFinding _instance;
    public static PathFinding Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        m_graph = new GraphNode[mapSize.y, mapSize.x];
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                m_graph[y, x] = new GraphNode();
            }
        }
    }

    private void reset()
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                m_graph[y, x].visited = false;
            }
        }

        m_adjacentPositions.Clear();
    }

    public void getDiagonalAdjacentPositions(List<Vector2Int> adjacentPositions, Vector2Int position, Point[,] map)
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        foreach(Vector2Int direction in Utilities.getDiagonalDirections2D())
        {
            Vector2Int positionOnGrid = position + direction;
            if (positionOnGrid.x >= 0 && positionOnGrid.x < mapSize.x &&
                positionOnGrid.y >= 0 && positionOnGrid.y < mapSize.y &&
                !map[positionOnGrid.y, positionOnGrid.x].visited &&
                fGameManager.Instance.isPointOnScenery(positionOnGrid))
            {
                adjacentPositions.Add(positionOnGrid);
            }
        }
    }

    public void getAdjacentPositions(List<Vector2Int> adjacentPositions, Vector2Int position, Point[,] map)
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        foreach (Vector2Int direction in m_directions2D)
        {
            Vector2Int positionOnGrid = position + direction;
            if (positionOnGrid.x >= 0 && positionOnGrid.x < mapSize.x &&
                positionOnGrid.y >= 0 && positionOnGrid.y < mapSize.y &&
                !map[positionOnGrid.y, positionOnGrid.x].visited &&
                fGameManager.Instance.isPointOnScenery(positionOnGrid))
            {
                adjacentPositions.Add(positionOnGrid);
            }
        }
    }
        
    public void getAdjacentPositions(List<Vector2Int> adjacentPositions, Vector2Int position)
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        foreach(Vector2Int direction in m_directions2D)
        {
            Vector2Int positionOnGrid = position + direction;
            if (positionOnGrid.x >= 0 && positionOnGrid.x < mapSize.x && 
                positionOnGrid.y >= 0 && positionOnGrid.y < mapSize.y)
            {
                adjacentPositions.Add(positionOnGrid);
            }
        }
    }

    public Vector3 getClosestSafePosition(Vector3 position, int minDistance, eFactionName factionName)
    {
        reset();
        Queue<FrontierNode> frontier = new Queue<FrontierNode>();
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
        frontier.Enqueue(new FrontierNode(positionOnGrid, 1));

        Vector3 safePosition = new Vector3();
        bool safePositionFound = false;
        while (!safePositionFound && frontier.Count > 0)
        {
            FrontierNode lastPosition = frontier.Dequeue();
            getAdjacentPositions(m_adjacentPositions, lastPosition.position);
            foreach (Vector2Int adjacentPosition in m_adjacentPositions)
            {
                if (m_graph[adjacentPosition.y, adjacentPosition.x].visited)
                {
                    continue;
                }

                m_graph[adjacentPosition.y, adjacentPosition.x].visited = true;
                frontier.Enqueue(new FrontierNode(adjacentPosition, lastPosition.depth + 1));

                if (Vector3.Distance(new Vector3(adjacentPosition.x, 0, adjacentPosition.y), position) >= Mathf.Abs(minDistance))
                {
                    safePositionFound = true;
                    safePosition = new Vector3(adjacentPosition.x, 0, adjacentPosition.y);
                    break;
                }
            }



            m_adjacentPositions.Clear();
        }

        return safePosition;
    }
}