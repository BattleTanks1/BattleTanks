using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public Faction[] m_factions;

    private int m_ID = 0; //Unique ID

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

        foreach(Faction faction in m_factions)
        {
            Assert.IsNotNull(faction);
        }
    }

    public Tank getTank(int ID)
    {
        foreach (Faction faction in m_factions)
        {
            foreach (Tank tank in faction.m_tanks)
            {
                if(tank.m_ID == ID)
                {
                    return tank;
                }
            }
        }

        return null;
    }

    public Tank getTank(Vector3 position)
    {
        foreach(Faction faction in m_factions)
        {
            foreach(Tank tank in faction.m_tanks)
            {
                Selection tankSelection = tank.GetComponent<Selection>();
                Assert.IsNotNull(tankSelection);
                if(tankSelection.contains(position))
                {
                    return tank;
                }
            }
        }

        return null;
    }

    public Faction getPlayerFaction()
    {
        Faction playerFaction = null;
        foreach(Faction faction in m_factions)
        {
            if(faction.getControllerType() == eFactionControllerType.Human)
            {
                playerFaction = faction;
            }
        }

        return playerFaction;
    }

    public int addUnit()
    {
        int ID = m_ID;
        ++m_ID;

        return ID;
    }

    public Vector3 getTankPosition(int tankID)
    {
        Assert.IsTrue(tankID != Utilities.INVALID_ID);

        foreach (Faction faction in m_factions)
        {
            foreach(Tank tank in faction.m_tanks)
            {
                if(tank.m_ID == tankID)
                {
                    return tank.transform.position;
                }
            }
        }

        return Utilities.INVALID_POSITION;
    }

    public void damageTank(Tank tank, int damage)
    {
        Assert.IsNotNull(tank);
        tank.reduceHealth(damage);
        
        if(tank.isDead())
        {
            Map.Instance.remove(tank);
            m_factions[(int)tank.m_factionName].m_tanks.Remove(tank);
            Destroy(tank.gameObject);
        }
    }
}