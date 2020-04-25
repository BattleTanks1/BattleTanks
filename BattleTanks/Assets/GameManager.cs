using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public Faction[] m_factions;
    [SerializeField]
    private List<Resource> m_resources;

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

    public Unit getUnit(int ID)
    {
        foreach (Faction faction in m_factions)
        {
            foreach (Unit unit in faction.m_unit)
            {
                if(unit.m_ID == ID)
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
            foreach(Unit unit in faction.m_unit)
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
            foreach(Unit unit in faction.m_unit)
            {
                if(unit.m_ID == tankID)
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
            Map.Instance.clear(unit.transform.position, unit.m_ID);
            m_factions[(int)unit.m_factionName].m_unit.Remove(unit);
            Destroy(unit.gameObject);
        }
    }

    public void addResource(Resource newResource)
    {
        Assert.IsNotNull(newResource);
        m_resources.Add(newResource);
    }

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
}