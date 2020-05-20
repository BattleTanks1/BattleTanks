using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Faction[] m_factions;
    [SerializeField]
    private List<Resource> m_resources;
    [SerializeField]
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

        m_resources = new List<Resource>();
    }

    public List<Unit> getOpposingFactionUnits(eFactionName sendingFaction)
    {
        switch (sendingFaction)
        {
            case eFactionName.Red:
                return m_factions[(int)eFactionName.Blue].m_units;
            case eFactionName.Blue:
                return m_factions[(int)eFactionName.Red].m_units;
            default:
                Assert.IsTrue(false);
                return null;
        }
    }

    public Unit getUnit(int ID)
    {
        foreach (Faction faction in m_factions)
        {
            foreach (Unit unit in faction.m_units)
            {
                if(unit.getID() == ID)
                {
                    return unit;
                }
            }
        }

        return null;
    }

    public Unit getUnit(Vector3 position)
    {
        foreach(Faction faction in m_factions)
        {
            foreach(Unit unit in faction.m_units)
            {
                Selection tankSelection = unit.GetComponent<Selection>();
                Assert.IsNotNull(tankSelection);
                if(tankSelection.contains(position))
                {
                    return unit;
                }
            }
        }

        return null;
    }

    public void getFactionUnitsInRange(ref List<int> output,Vector3 position, float range, int faction)
    {
        for (int i = 0; i < m_factions[faction].m_units.Count; ++i)
        {
            Vector3 diff = m_factions[faction].m_units[i].getPosition() - position;
            if (diff.sqrMagnitude <= range * range && diff.sqrMagnitude != 0)
            {
                output.Add(m_factions[faction].m_units[i].getID());
            }
        }
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
            foreach(Unit unit in faction.m_units)
            {
                if(unit.getID() == tankID)
                {
                    return unit.transform.position;
                }
            }
        }

        return Utilities.INVALID_POSITION;
    }

    public void damageUnit(Unit unit, int damage)
    {
        Assert.IsNotNull(unit);
        unit.reduceHealth(damage);
        
        if(unit.isDead())
        {
            m_factions[(int)unit.getFactionName()].removeUnit(unit);
        }
    }

    public void addResource(Resource newResource)
    {
        Assert.IsNotNull(newResource);
        m_resources.Add(newResource);
    }

    //public BoidSpawner getFactionBoidSpawner(eFactionName factionName)
    //{
    //    BoidSpawner boidSpawner = null;
    //    switch (factionName)
    //    {
    //        case eFactionName.Red:
    //        {
    //            m_factions[(int)factionName]
    //        }
    //        break;
    //    }


    //    return boidSpawner;
    //    foreach (Resource resource in m_resources)
    //    {
    //        Selection selection = resource.GetComponent<Selection>();
    //        Assert.IsNotNull(selection);

    //        if (selection.contains(position))
    //        {
    //            return resource;
    //        }
    //    }
    //}

    public Resource getResource(Vector3 position)
    {
        foreach (Resource resource in m_resources)
        {
            Selection selection = resource.GetComponent<Selection>();
            Assert.IsNotNull(selection);

            if(selection.contains(position))
            {
                return resource;
            }
        }

        return null;
    }

    public void createInfluence(FactionInfluenceMap[] proximityMaps, FactionInfluenceMap[] threatMaps)
    {
        Assert.IsNotNull(proximityMaps);
        Assert.IsNotNull(threatMaps);

        foreach (Faction faction in m_factions)
        {
            foreach (Unit unit in faction.m_units)
            {
                unit.createInfluence(proximityMaps, threatMaps);
            }
        }
    }

    public void addResourcesToFaction(Harvester harvester)
    {
        Unit unit = harvester.GetComponent<Unit>();
        Assert.IsNotNull(unit);

        m_factions[(int)unit.getFactionName()].addResources(harvester);
    }
}