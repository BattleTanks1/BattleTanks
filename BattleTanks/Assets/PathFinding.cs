using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool[,] m_graph;

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
        Vector2Int mapSize = Map.Instance.m_mapSize;
        m_graph = new bool[mapSize.y, mapSize.x];
        reset();
    }

    private void reset()
    {
        Vector2Int mapSize = Map.Instance.m_mapSize;
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                m_graph[y, x] = false;
            }
        }

        m_adjacentPositions.Clear();
    }

        
    public void getAdjacentPositions(List<Vector2Int> adjacentPositions, Vector2Int position)
    {
        Vector2Int mapSize = Map.Instance.m_mapSize;
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

    public Vector3 getClosestSafePosition(int minDistance, Unit unit)
    {
        reset();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(unit.transform.position);
        frontier.Enqueue(positionOnGrid);

        Vector3 safePosition = new Vector3();
        bool safePositionFound = false;
        while (!safePositionFound && frontier.Count > 0)
        {
            Vector2Int lastPosition = frontier.Dequeue();
            getAdjacentPositions(m_adjacentPositions, lastPosition);
            foreach (Vector2Int adjacentPosition in m_adjacentPositions)
            {
                //Visited
                if (m_graph[adjacentPosition.y, adjacentPosition.x])
                {
                    continue;
                }

                m_graph[adjacentPosition.y, adjacentPosition.x] = true;
                frontier.Enqueue(adjacentPosition);

                if (Vector3.Distance(new Vector3(adjacentPosition.x, 0, adjacentPosition.y), unit.transform.position) >= minDistance &&
                    InfluenceMap.Instance.isPositionInThreat(unit))
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