﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PointOnMap
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
        sceneryType = eSceneryType.None;
        tankID = Utilities.INVALID_ID;
    }

    public eSceneryType sceneryType;
    public int tankID = Utilities.INVALID_ID;
    public eFactionName tankFactionName;
}

public class Map : MonoBehaviour
{
    public Vector2Int m_mapSize { get; private set; }

    private PointOnMap[,] m_map;

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

        m_mapSize = new Vector2Int(250, 250);
        m_map = new PointOnMap[m_mapSize.y, m_mapSize.x];
        for (int y = 0; y < m_mapSize.y; ++y)
        {
            for (int x = 0; x < m_mapSize.x; ++x)
            {
                m_map[y, x] = new PointOnMap();
            }
        }
    }

    public bool isInBounds(int x, int y)
    {
        return x >= 0 &&
            x < m_mapSize.x &&
            y >= 0 &&
            y < m_mapSize.y;
    }

    public bool isInBounds(Vector2Int position)
    {
        return position.x >= 0 &&
            position.x < m_mapSize.x &&
            position.y >= 0 &&
            position.y < m_mapSize.y;
    }

    public bool isInBounds(Vector3 position)
    {
        return position.x >= 0 &&
            position.x < m_mapSize.x &&
            position.z >= 0 &&
            position.z < m_mapSize.y;
    }

    public void setStartingPosition(Vector3 startingPosition, eFactionName factionName, int ID)
    {
        Assert.IsTrue(isInBounds(startingPosition));
        if (isInBounds(startingPosition))
        {
            Vector2Int startingPositionOnGrid = Utilities.convertToGridPosition(startingPosition);
            Assert.IsTrue(m_map[startingPositionOnGrid.y, startingPositionOnGrid.x].isEmpty());
            m_map[startingPositionOnGrid.y, startingPositionOnGrid.x].assign(ID, factionName);
        }
    }

    public void updatePositionOnMap(Vector3 oldPosition, Vector3 currentPosition, eFactionName factionName, int ID)
    { 
        Assert.IsTrue(isInBounds(currentPosition));
        if(isInBounds(currentPosition))
        {
            Vector2Int oldPositionOnGrid = Utilities.convertToGridPosition(oldPosition);
            Assert.IsTrue(isPositionOccupied(oldPosition, ID));
            Vector2Int currentPositionOnGrid = Utilities.convertToGridPosition(currentPosition);
            Assert.IsTrue(oldPositionOnGrid != currentPositionOnGrid);

           
            m_map[oldPositionOnGrid.y, oldPositionOnGrid.x].reset();
            Assert.IsTrue(m_map[currentPositionOnGrid.y, currentPositionOnGrid.x].isEmpty());
            m_map[currentPositionOnGrid.y, currentPositionOnGrid.x].assign(ID, factionName);
        }
    }

    public bool isPositionOnOccupiedCell(Vector3 currentPosition, Vector3 newPosition)
    {
        Assert.IsTrue(isInBounds(currentPosition));
        Assert.IsTrue(isInBounds(newPosition));
        if(isInBounds(newPosition))
        {
            Vector2Int currentPositionOnGrid = Utilities.convertToGridPosition(currentPosition);
            Vector2Int newPositionOnGrid = Utilities.convertToGridPosition(newPosition);

            return currentPositionOnGrid == newPositionOnGrid;
        }
        else
        {
            return true;
        }
    }

    public bool isPositionOnCurrentCell(Vector3 position, int ID)
    {
        Assert.IsTrue(isInBounds(position));
        if (isInBounds(position))
        {
            Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
            return m_map[positionOnGrid.y, positionOnGrid.x].tankID == ID;
        }
        else
        {
            return true;
        }
    }

    public bool isPositionOccupied(Vector3 position, int senderID)
    {
        Assert.IsTrue(isInBounds(position));
        if (isInBounds(position))
        {
            Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
            return m_map[positionOnGrid.y, positionOnGrid.x].tankID == senderID;
        }
        else
        {
            return true;
        }
    }

    public bool isPositionOccupied(Vector3 position)
    {
        Assert.IsTrue(isInBounds(position));
        if(isInBounds(position))
        {
            Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
            return m_map[positionOnGrid.y, positionOnGrid.x].sceneryType != eSceneryType.None ||
                 m_map[positionOnGrid.y, positionOnGrid.x].tankID != Utilities.INVALID_ID;
           
        }
        else
        {
            return true;
        }
    }

    public bool isEnemyOnPosition(Vector2Int position, eFactionName factionName, out int targetID)
    {
        if (isInBounds(position) && m_map[position.y, position.x].tankID != Utilities.INVALID_ID)
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
        if (isInBounds(position))
        {
            return m_map[position.y, position.x].sceneryType != eSceneryType.None;
        }
        else
        {
            return false;
        }
    }

    public PointOnMap getPointOnMap(Vector2Int position)
    {
        if(isInBounds(position))
        {
            return m_map[position.y, position.x];
        }
        else
        {
            return null;
        }
    }

    public PointOnMap getPointOnMap(int x, int y)
    {
        if(isInBounds(x, y))
        {
            return m_map[y, x];
        }
        else
        {
            return null;
        }
    }

    public void addScenery(iRectangle rect, eSceneryType type)
    {
        for (int y = rect.m_top; y <= rect.m_bottom; ++y)
        {
            for (int x = rect.m_left; x <= rect.m_right; ++x)
            {
                Assert.IsTrue(isInBounds(x, y));
                if (isInBounds(x, y))
                {
                    m_map[y, x].sceneryType = type;
                }
            }
        }
    }

    public bool isPositionScenery(int x, int y)
    {
        Assert.IsTrue(isInBounds(x, y));
        if (isInBounds(x, y))
        {
            return m_map[y, x].sceneryType != eSceneryType.None;
        }
        else
        {
            return false;
        }
    }

    public void remove(Tank tank)
    {
        Assert.IsNotNull(tank);
        if(tank)
        {
            Vector2Int positionOnGrid = Utilities.convertToGridPosition(tank.transform.position);
            Assert.IsTrue(isInBounds(positionOnGrid));
            if (isInBounds(positionOnGrid))
            {
                m_map[positionOnGrid.y, positionOnGrid.x].reset();
            }
        }
    }
}
