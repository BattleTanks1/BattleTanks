using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fGameManager : MonoBehaviour
{
    public Faction[] m_factions { get; private set; }
    //spublic List<Faction> m_factions { get; private set; }
    [SerializeField]
    public Vector2Int m_mapSize;
    private int m_ID = 0;

    public PlayerTank m_player;

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
            new Faction(eFactionName.Red);
            new Faction(eFactionName.Blue);
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
}