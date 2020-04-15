using System.Collections;
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

public class GameManager : MonoBehaviour
{
    public Faction[] m_factions;
    public Vector2Int m_mapSize { get; private set; }
    private int m_ID = 0; //Unique ID per ship
    private GraphPoint[,] m_map;
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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

        m_factions = new Faction[(int)eFactionName.Total];
        m_factions[(int)eFactionName.Red] = new FactionHuman(eFactionName.Red);
        m_factions[(int)eFactionName.Blue] = new FactionAI(eFactionName.Blue);

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

    private void Update()
    {
        foreach(Faction faction in m_factions)
        {
            faction.update();
        }
    }

    private Faction getPlayerFaction()
    {
        Faction playerFaction = null;
        foreach(Faction faction in m_factions)
        {
            if(faction.m_controllerType == eFactionControllerType.eHuman)
            {
                playerFaction = faction;
            }
        }

        return playerFaction;
    }

    public int addTank(Tank tank)
    {
        int ID = m_ID;
        ++m_ID;
        switch (tank.m_factionName)
        {
            case eFactionName.Red:
                m_factions[(int)tank.m_factionName].addTank(tank);
                break;
            case eFactionName.Blue:
                m_factions[(int)tank.m_factionName].addTank(tank);
                break;
        }

        return ID;
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
        if(m_map[position.y, position.x].tankID != Utilities.INVALID_ID)
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

    public void sendAIControllerMessage(MessageToAIController message)
    {
        switch (message.m_senderFaction)
        {
            case eFactionName.Red:
                {
                    FactionAI faction = m_factions[(int)message.m_senderFaction] as FactionAI;
                    faction.addMessage(message);
                }
                break;
            case eFactionName.Blue:
                {
                    FactionAI faction = m_factions[(int)message.m_senderFaction] as FactionAI;
                    faction.addMessage(message);
                }
                break;
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
        for(int y = rect.m_top; y <= rect.m_bottom; ++y)
        {
            for(int x = rect.m_left; x <= rect.m_right; ++x)
            {
                m_map[y, x].sceneryType = type;
            }
        }
    }

    public bool isPositionScenery(int x, int y)
    {
        return m_map[y, x].sceneryType != eSceneryType.None;
    }

    public void selectPlayerUnits(fRectangle selectionBox)
    {
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        if(playerFaction == null)
        {
            return;
        }

        foreach (Tank tank in playerFaction.m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            if(tankSelection)
            {
                tankSelection.Select(selectionBox);
            }
        }
    }

    public void deselectPlayerUnits()
    {
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        if (playerFaction == null)
        {
            return;
        }

        foreach (Tank tank in playerFaction.m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            if (tankSelection)
            {
                tankSelection.Deselect();
            }
        }
    }

    public void moveSelectedPlayerUnitsToPosition(Vector3 position)
    {
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        if (playerFaction == null)
        {
            return;
        }

        foreach(Tank tank in playerFaction.m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            TankMovement tankMovement = tank.gameObject.GetComponent<TankMovement>();
            Assert.IsNotNull(tankMovement);

            if(tankSelection.isSelected())
            {
                tankMovement.moveTo(position);
            }
        }
    }
}