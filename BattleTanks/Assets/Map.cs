﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GraphPoint
{
    public bool isEmpty()
    {
        return tankID == Utilities.INVALID_ID;
    }

    public void assign(int ID, eFactionName factionName)
    {
        tankID = ID;
        tankFactionName = factionName;
    }

    public void reset()
    {
        tankID = Utilities.INVALID_ID;
    }

    public eSceneryType sceneryType;
    public int tankID = Utilities.INVALID_ID;
    public eFactionName tankFactionName;
}

public class Map : MonoBehaviour
{
    public Vector2Int m_mapSize { get; private set; }

    private GraphPoint[,] m_map;

    private static Map _instance;
    public static Map Instance { get { return _instance; } }

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

        m_mapSize = new Vector2Int(128, 128);
        m_map = new GraphPoint[m_mapSize.y, m_mapSize.x];
        for (int y = 0; y < m_mapSize.y; ++y)
        {
            for (int x = 0; x < m_mapSize.x; ++x)
            {
                m_map[y, x] = new GraphPoint();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updatePositionOnMap(Vector3 oldPosition, Vector3 position, eFactionName factionName, int ID)
    {
        Vector2Int oldPositionOnGrid = Utilities.convertToGridPosition(oldPosition);
        Vector2Int currentPositionOnGrid = Utilities.convertToGridPosition(position);

        if (oldPositionOnGrid != currentPositionOnGrid)
        {
            m_map[oldPositionOnGrid.y, oldPositionOnGrid.x].reset();
            Assert.IsTrue(m_map[currentPositionOnGrid.y, currentPositionOnGrid.x].isEmpty());
            m_map[currentPositionOnGrid.y, currentPositionOnGrid.x].assign(ID, factionName);
        }
    }

    public bool isPositionOccupied(Vector3 newPosition, int tankID)
    {
        Vector2Int newPositionOnGrid = Utilities.convertToGridPosition(newPosition);
        if (m_map[newPositionOnGrid.y, newPositionOnGrid.x].sceneryType != eSceneryType.None)
        {
            return true;
        }
        //Tank moving in same grid
        else if (m_map[newPositionOnGrid.y, newPositionOnGrid.x].tankID == tankID)
        {
            return false;
        }
        else
        {
            return m_map[newPositionOnGrid.y, newPositionOnGrid.x].tankID != Utilities.INVALID_ID;
        }
    }

    public bool isEnemyOnPosition(Vector2Int position, eFactionName factionName, out int targetID)
    {
        if (m_map[position.y, position.x].tankID != Utilities.INVALID_ID)
        {
            targetID = m_map[position.y, position.x].tankID;
            return m_map[position.y, position.x].tankFactionName != factionName;
        }
        else
        {
            targetID = Utilities.INVALID_ID;
            return false;
        }
    }

    public bool isPointOnScenery(Vector2Int position)
    {
        return m_map[position.y, position.x].sceneryType != eSceneryType.None;
    }

    public GraphPoint getPointOnMap(Vector2Int position)
    {
        return m_map[position.y, position.x];
    }

    public GraphPoint getPointOnMap(int y, int x)
    {
        return m_map[y, x];
    }

    public void addScenery(iRectangle rect, eSceneryType type)
    {
        for (int y = rect.m_top; y <= rect.m_bottom; ++y)
        {
            for (int x = rect.m_left; x <= rect.m_right; ++x)
            {
                m_map[y, x].sceneryType = type;
            }
        }
    }

    public bool isPositionScenery(int x, int y)
    {
        return m_map[y, x].sceneryType != eSceneryType.None;
    }
}
