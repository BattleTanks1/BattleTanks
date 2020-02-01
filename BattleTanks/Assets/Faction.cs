using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eFactionName
{
    Red = 0,
    Blue,
    Total
}

public enum eFactionControllerType
{
    eHuman = 0,
    eAI
}

abstract public class Faction
{
    public List<Tank> m_tanks;
    public eFactionName m_name { get; private set; }
    public eFactionControllerType m_controllerType { get; private set; }

    public Faction(eFactionName name, eFactionControllerType controllerType)
    {
        m_name = name;
        m_controllerType = controllerType;
        m_tanks = new List<Tank>();
    }

    public void addTank(Tank tank)
    {
        m_tanks.Add(tank);
    }
}

public class FactionHuman : Faction
{
    public FactionHuman(eFactionName name) :
        base(name, eFactionControllerType.eHuman)
    { }
}