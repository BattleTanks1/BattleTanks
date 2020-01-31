using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eAIUniMessageType
{
    EnemySpottedAtPosition = 0
}

public class AIUnitMessage
{
   
}

public class GraphPoint
{
    public void assign(int ID, eFactionName factionName)
    {
        tankID = ID;
        tankFactionName = factionName;
    }

    public void reset()
    {
        tankID = -1;
    }

    public int tankID = -1;
    public eFactionName tankFactionName;
}

public class fGameManager : MonoBehaviour
{
    public Faction[] m_factions { get; private set; }
    [SerializeField]
    public Vector2Int m_mapSize;
    private int m_ID = 0; //Unique ID per ship
    public GraphPoint[,] m_map { get; private set; }

    private static fGameManager _instance;
    public static fGameManager Instance { get { return _instance; } }

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

        Faction[] m_factions = new Faction[(int)eFactionName.Total];
        {
            new FactionAI(eFactionName.Red);
            new FactionAI(eFactionName.Blue);
        }

        m_map = new GraphPoint[m_mapSize.y, m_mapSize.x];
        for(int y = 0; y < m_mapSize.y; ++y)
        {
            for(int x = 0; x < m_mapSize.x; ++x)
            {
                m_map[y, x] = new GraphPoint();        
            }
        }

        m_mapSize = new Vector2Int(128, 128);
    }

    private void Update()
    {

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

    public void updatePositionOnMap(Tank tank)
    {
        Vector2Int oldPositionOnGrid = Utilities.getPositionOnGrid(tank.m_oldPosition);
        Vector2Int currentPositionOnGrid = Utilities.getPositionOnGrid(tank.transform.position);

        if (oldPositionOnGrid != currentPositionOnGrid)
        {
            m_map[oldPositionOnGrid.y, oldPositionOnGrid.x].reset();
        }
   
        m_map[currentPositionOnGrid.y, currentPositionOnGrid.x].assign(tank.m_ID, tank.m_factionName);
    }
}