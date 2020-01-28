using System.Collections.Generic;
using UnityEngine;

public class GraphPoint
{
    public bool visited = false;
}

public class AIHandler : MonoBehaviour
{
    GraphPoint[,] m_map;

    // Start is called before the first frame update
    void Start()
    {
        Vector2Int mapSize = InfluenceMap.Instance.mapSize;
        m_map = new GraphPoint[mapSize.y, mapSize.x];
        for(int y= 0; y < mapSize.y; ++y)
        {
            for(int x = 0; x < mapSize.x; ++x)
            {
                m_map[y, x] = new GraphPoint();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    List<Vector2Int> getAdjacentPositions(Vector2Int position)
    {
        Vector2Int mapSize = InfluenceMap.Instance.mapSize;
        List<Vector2Int> adjacentPositions = new List<Vector2Int>();
        for (int x = position.x - 1; x <= position.x + 1; x += 2)
        {
            if (x >= 0 && x < mapSize.x)
            {
                adjacentPositions.Add(new Vector2Int(x, position.y));
            }
        }

        for (int y = position.y - 1; y <= position.y + 1; y += 2)
        {
            if (y >= 0 && y < mapSize.y)
            {
                adjacentPositions.Add(new Vector2Int(position.x, y));
            }
        }

        return adjacentPositions;
    }

    public Vector3 getClosestSafePosition(Vector3 position)
    {
        Vector2Int positionOnGrid = new Vector2Int((int)Mathf.Abs(Mathf.Round(position.x)), (int)Mathf.Abs(Mathf.Round(position.z)));
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(positionOnGrid);

        bool targetFound = false;
        while(!targetFound && frontier.Count > 0)
        {
            Vector2Int lastPosition = frontier.Dequeue();
            foreach(Vector2Int adjacentPosition in getAdjacentPositions(lastPosition))
            {
                if(m_map[adjacentPosition.y, adjacentPosition.x].visited)
                {
                    continue;
                }

                m_map[adjacentPosition.y, adjacentPosition.x].visited = true;
                frontier.Enqueue(adjacentPosition);

                if(InfluenceMap.Instance.getPointOnThreatMap(adjacentPosition).value )


            }


        }

        return new Vector3();
    }
}