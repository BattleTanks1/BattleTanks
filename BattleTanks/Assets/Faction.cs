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


    public eFactionControllerType getControllerType()
    {
        return m_controllerType;
    }

    public eFactionName getFactionName()
    {
        return m_factionName;
    }
}