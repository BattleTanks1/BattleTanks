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
    public List<Unit> m_unit;

    [SerializeField]
    protected eFactionName m_factionName;
    protected eFactionControllerType m_controllerType;

    public eFactionControllerType getControllerType()
    {
        return m_controllerType;
    }

    public eFactionName getFactionName()
    {
        return m_factionName;
    }

    public void addUnit(Unit newUnit)
    {
        Assert.IsNotNull(newUnit);
        m_unit.Add(newUnit);
    }

    public void removeTank(Unit unit)
    {
        m_unit.Remove(unit);
        
        Destroy(unit.gameObject);
    }
}