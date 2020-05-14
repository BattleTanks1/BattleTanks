using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PointOnMap
{
    public bool isOccupiedByEnemy(eFactionName factionName)
    {
        return (unitID != Utilities.INVALID_ID && factionName != unitFactionName);
    }

    public bool isEmpty()
    {
        return unitID == Utilities.INVALID_ID && !scenery;
    }

    public void assign(int ID, eFactionName factionName)
    {
        unitID = ID;
        unitFactionName = factionName;
    }

    public void reset()
    {
        scenery = false;
        unitID = Utilities.INVALID_ID;
    }

    public bool scenery = false;
    public int unitID = Utilities.INVALID_ID;
    public eFactionName unitFactionName;
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

    private PointOnMap getPoint(Vector2Int position)
    {
        Assert.IsTrue(isInBounds(position));
        return m_map[position.y, position.x];
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

        Vector2Int startingPositionOnGrid = Utilities.convertToGridPosition(startingPosition);
        Assert.IsTrue(getPoint(startingPositionOnGrid).isEmpty());

        getPoint(startingPositionOnGrid).assign(ID, factionName);
    }

    public void updatePositionOnMap(Vector3 currentPosition, Vector3 oldPosition, eFactionName factionName, int ID)
    { 
        Assert.IsTrue(isInBounds(currentPosition));
        Assert.IsTrue(isInBounds(oldPosition));

        Vector2Int oldPositionOnGrid = Utilities.convertToGridPosition(oldPosition);
        Assert.IsTrue(isPositionOccupied(oldPosition, ID));
        Vector2Int currentPositionOnGrid = Utilities.convertToGridPosition(currentPosition);
        Assert.IsTrue(oldPositionOnGrid != currentPositionOnGrid);

        Assert.IsTrue(getPoint(currentPositionOnGrid).isEmpty());
        getPoint(oldPositionOnGrid).reset();
        getPoint(currentPositionOnGrid).assign(ID, factionName);
    }

    public bool isPositionOnOccupiedCell(Vector3 newPosition, Vector3 currentPosition)
    {
        Assert.IsTrue(isInBounds(currentPosition));
        Assert.IsTrue(isInBounds(newPosition));

        Vector2Int currentPositionOnGrid = Utilities.convertToGridPosition(currentPosition);
        Vector2Int newPositionOnGrid = Utilities.convertToGridPosition(newPosition);

        return currentPositionOnGrid == newPositionOnGrid;
    }

    public bool isPositionOccupied(Vector3 position, int senderID)
    {
        Assert.IsTrue(isInBounds(position));

        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
        return getPoint(positionOnGrid).unitID == senderID;
    }

    public bool isPositionOccupied(Vector3 position)
    {
        Assert.IsTrue(isInBounds(position));

        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
        return !getPoint(positionOnGrid).isEmpty();
    }

    public bool isEnemyOnPosition(Vector2Int position, eFactionName factionName, out int targetID)
    {
        Assert.IsTrue(isInBounds(position));
        if (getPoint(position).unitID != Utilities.INVALID_ID)
        {
            targetID = getPoint(position).unitID;
            return getPoint(position).unitFactionName != factionName;
        }
        else
        {
            targetID = Utilities.INVALID_ID;
            return false;
        }
    }

    public bool isPointOnScenery(Vector2Int position)
    {
        Assert.IsTrue(isInBounds(position));
        return getPoint(position).scenery;
    }

    public PointOnMap getPoint(int x, int y)
    {
        Assert.IsTrue(isInBounds(x, y));
        return m_map[y, x];
    }

    public void addScenery(iRectangle rect)
    {
        for (int y = rect.m_bottom; y <= rect.m_top; ++y)
        {
            for (int x = rect.m_left; x <= rect.m_right; ++x)
            {
                Assert.IsTrue(isInBounds(x, y));
                Assert.IsTrue(getPoint(x, y).isEmpty());
                getPoint(x, y).scenery = true;
            }
        }
        Pathfinder.Instance.updateObstructions(m_map);
    }

    public bool isPositionScenery(int x, int y)
    {
        Assert.IsTrue(isInBounds(x, y));
        return getPoint(x, y).scenery;
    }

    public void clear(Vector3 position, int senderID)
    {
        Assert.IsTrue(isInBounds(position));
        Assert.IsTrue(isPositionOccupied(position, senderID));

        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
        getPoint(positionOnGrid).reset();
    }
}