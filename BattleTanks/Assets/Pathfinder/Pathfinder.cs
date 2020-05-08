using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

//Thoughts: should be using a single map to reduce cache bouncing

public struct Node
{
    Vector2Int m_averagePosition;
    UnityEngine.Vector2 m_dangerLevel;
    float m_usageLevel;
}

public struct ExplorationNode
{
    public bool explored;
    public Vector2Int parent;
}

public class Pathfinder : MonoBehaviour
{
    private bool[,] m_validTiles;
    public ExplorationNode[,] m_exploredTiles;
    public Vector2Int m_mapSize;
    public SortedList<float, Vector2Int> m_searchList;

    private bool isTileValid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= m_mapSize.x || pos.y >= m_mapSize.y)
            return false;
        return m_validTiles[pos.x, pos.y];
    }

    private float getDistance(Vector2Int a, Vector2Int b)
    {
        if (a == b)
            return 0.0f;
        return Mathf.Sqrt(((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y)));
    }

    private float findWeight(Vector2Int tile, Vector2Int from, Vector2Int dest, int faction, float dangerAvoidance, float usageAvoidance)
    {
        //Cost to get there
        float weight = getDistance(tile, from);
        //Distance to the destination
        weight += getDistance(tile, dest);
        //Danger amount TODO

        //Tile usage amount TODO

        return weight;
    }

    private bool exploreTile(Vector2Int location, Vector2Int goal, 
        int faction, float dangerAvoidance, float usageAvoidance)
    {
        if (location == goal)
            return true;

        Vector2Int[] options = {
            new Vector2Int(location.x, location.y - 1),//North
            new Vector2Int(location.x + 1, location.y),//East
            new Vector2Int(location.x, location.y + 1),//South
            new Vector2Int(location.x - 1, location.y),//West
            new Vector2Int(location.x + 1, location.y - 1),//NorthEast
            new Vector2Int(location.x + 1, location.y + 1),//SouthEast
            new Vector2Int(location.x - 1, location.y + 1),//SouthWest
            new Vector2Int(location.x - 1, location.y - 1),//NorthWest
        };
        //Add each valid adjacent to list
        int count = 0;
        foreach (Vector2Int option in options)
        {
            //TODO special cases for diagonals?
            if (isTileValid(option) && !m_exploredTiles[option.x, option.y].explored)
            {
                m_exploredTiles[options[count].x, options[count].y].explored = true;
                m_exploredTiles[options[count].x, options[count].y].parent = location;

                float weight = findWeight(options[count], location, goal, faction, dangerAvoidance, usageAvoidance);
                while (m_searchList.ContainsKey(weight))
                    weight += 0.01f;
                m_searchList.Add(weight, options[count]);
            }
            count++;
        }
        return false;
    }

    Queue<UnityEngine.Vector2> findPath(Vector2Int start, Vector2Int destination, int faction, float dangerAvoidance, float usageAvoidance)
    {
        //Clear exploration map
        for (int i = 0; i < m_mapSize.x; ++i)
        {
            for (int j = 0; j < m_mapSize.y; ++j)
            {
                m_exploredTiles[i, j].explored = false;
            }
        }

        //Start recursive search
        m_searchList.Clear();
        m_searchList.Add(0, destination);
        while (m_searchList.Count != 0)
        {
            //Run search on tile
            KeyValuePair<float, Vector2Int> tile = m_searchList.First();
            m_searchList.RemoveAt(0);
            if (exploreTile(tile.Value, start, faction, dangerAvoidance, usageAvoidance))
                break;
        }
        //TODO trace the exploration list and smooth path list
    }

    // Start is called before the first frame update
    void Awake()
    {
        //Create obstruction map and high level map
    }

    // Update is called once per frame
    void Update()
    {
        //Update macro map with influence maps

        //Remove items from the pathing queue up to a limit

        //Decay danger and usage levels
    }
}
