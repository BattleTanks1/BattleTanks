using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
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
    public bool obstructed;
    public bool explored;
    public Vector2Int parent;
    public float dangerInfluence;
    public float usageInfluence;
}

public class Pathfinder : MonoBehaviour
{
    public ExplorationNode[,] m_exploredTiles;
    public Vector2Int m_mapSize = new Vector2Int(250, 250);
    public SortedList<float, Vector2Int> m_searchList;

    private bool isTileValid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= m_mapSize.x || pos.y >= m_mapSize.y)
            return false;
        return !m_exploredTiles[pos.x, pos.y].obstructed;
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
        weight += m_exploredTiles[tile.x, tile.y].dangerInfluence * faction * dangerAvoidance;
        //Tile usage amount TODO
        weight += m_exploredTiles[tile.x, tile.y].usageInfluence * usageAvoidance;

        return weight;
    }

    private bool isInline(Vector2Int a, Vector2Int b, Vector2Int c)
    {
        UnityEngine.Vector2 ab = b - a;
        UnityEngine.Vector2 ac = c - a;

        if (ab.magnitude == 0.0f || ac.magnitude == 0.0f)
            return false;

        float product = UnityEngine.Vector2.Dot(ab, ac) / (ab.magnitude * ac.magnitude);
        return (product == 1.0f);
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

    public void updateObstructions(in bool[,] obstructions)
    {
        for (int i = 0; i < m_mapSize.x; ++i)
        {
            for (int j = 0; j < m_mapSize.y; ++j)
            {
                m_exploredTiles[i, j].obstructed = obstructions[i, j];
            }
        }
    }

    public Queue<Vector2Int> findPath(Vector2Int start, Vector2Int destination, int faction, float dangerAvoidance, float usageAvoidance)
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
        bool success = false;
        while (m_searchList.Count != 0)
        {
            //Run search on tile
            KeyValuePair<float, Vector2Int> tile = m_searchList.First();
            m_searchList.RemoveAt(0);
            if (exploreTile(tile.Value, start, faction, dangerAvoidance, usageAvoidance))
            {
                success = true;
                break;
            }
        }

        //Create the list of points the unit must travel through
        Queue<Vector2Int> path = new Queue<Vector2Int>();
        if (success)
        {
            Vector2Int currentLoc = start;
            Vector2Int lastLoc = new Vector2Int(-1, -1);
            Vector2Int secondLastLoc = start;
            //Loop creating path
            while (currentLoc != destination)
            {
                currentLoc = m_exploredTiles[currentLoc.x, currentLoc.y].parent;

                //Check for a linear streak, and cull unnecessary path points
                if (isInline(secondLastLoc, lastLoc, currentLoc))
                    path.Dequeue();

                //Set up for next search
                lastLoc = currentLoc;
                if (path.Count != 0)
                    secondLastLoc = path.Peek();

                path.Enqueue(currentLoc);
            }
        }
        return path;
    }

    // Start is called before the first frame update
    void Awake()
    {
        //Create exploration map
        m_exploredTiles = new ExplorationNode[250, 250];
    }

    // Update is called once per frame
    void Update()
    {
        //Update exploration map with influence maps

        //Remove items from the pathing queue up to a limit TODO: create a pathing queue

        //Decay usage level
    }
}
