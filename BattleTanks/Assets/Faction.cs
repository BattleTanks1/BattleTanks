using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eFactionName
{
    Red = 0,
    Blue,
    Total
}

public enum eFactionControllerType
{
    Human = 0,
    AI
}

abstract public class Faction : MonoBehaviour
{
    public List<Unit> m_units;

    [SerializeField]
    protected Building m_building = null;
    [SerializeField]
    protected eFactionName m_factionName;
    protected eFactionControllerType m_controllerType;
    [SerializeField]
    private int m_resourceCount = 0;

    private void Awake()
    {
        m_units = new List<Unit>();
    }

    public void spawnUnit(eUnitType unitType)
    {
        Selection buildingSelection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(buildingSelection);

        if (buildingSelection.isSelected())
        {
            Unit newUnit = m_building.spawnUnit(unitType);
            if (newUnit)
            {
                addUnit(newUnit);
            }
        }
    }

    public eFactionControllerType getControllerType()
    {
        return m_controllerType;
    }

    public eFactionName getFactionName()
    {
        return m_factionName;
    }

    public void addResources(Harvester harvester)
    {
        Assert.IsNotNull(harvester);
        m_resourceCount += harvester.extractResources();
    }

    public void addUnit(Unit newUnit)
    { 
        Assert.IsNotNull(newUnit);
        Assert.IsTrue(newUnit.getID() == Utilities.INVALID_ID);

        m_units.Add(newUnit);
    }

    public void removeUnit(Unit unit)
    {
        Assert.IsNotNull(unit);

        Map.Instance.clear(unit.transform.position, unit.getID());
        
        bool removed = m_units.Remove(unit);
        Assert.IsTrue(removed);

        Destroy(unit.gameObject);
    }
}