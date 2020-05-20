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
    protected int m_startingHarvesterCount = 2;
    [SerializeField]
    protected BoidSpawner m_boidSpawner = null;
    [SerializeField]
    protected Building m_building = null;
    [SerializeField]
    protected eFactionName m_factionName;
    protected eFactionControllerType m_controllerType;
    [SerializeField]
    private int m_resourceCount = 0;

    protected virtual void Awake()
    {
        Assert.IsNotNull(m_boidSpawner);
        Assert.IsNotNull(m_building);

        m_units = new List<Unit>();
    }

    protected virtual void Start()
    {
        for (int i = 0; i < 2; ++i)
        {
            Unit harvester = m_building.spawnUnit(eUnitType.Harvester);
            addUnit(harvester);

            HarvesterStateHandler harvesterStateHandler = harvester.GetComponent<HarvesterStateHandler>();
            Assert.IsNotNull(harvesterStateHandler);
            harvesterStateHandler.switchToState(eHarvesterState.SetBoidSpawner, m_boidSpawner);
            harvesterStateHandler.switchToState(eHarvesterState.TargetAvailableBoid);
        }
    }

    protected virtual void Update()
    {}

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

    public void addResources(Harvester harvester)
    {
        Assert.IsNotNull(harvester);
       // m_resourceCount += harvester.extractResources();
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

        bool removed = m_units.Remove(unit);
        Assert.IsTrue(removed);

        Destroy(unit.gameObject);
    }
}