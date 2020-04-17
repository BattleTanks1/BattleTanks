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
    Human = 0,
    AI
}

abstract public class Faction : MonoBehaviour
{
    public List<Tank> m_tanks;

    [SerializeField]
    protected eFactionName m_factionName;
    protected eFactionControllerType m_controllerType;

    private void Awake()
    {
        m_tanks = new List<Tank>();
    }

    public eFactionControllerType getControllerType()
    {
        return m_controllerType;
    }

    public eFactionName getFactionName()
    {
        return m_factionName;
    }

    public void addTank(Tank tank)
    {
        m_tanks.Add(tank);
    }
}

//public class FactionHuman : Faction
//{
//    public FactionHuman(eFactionName name) :
//        base(name, eFactionControllerType.Human)
//    { }
//}