using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eFactionName
{
    Red = 0,
    Blue,
    Total
}

public class Faction : MonoBehaviour
{
    public Faction(eFactionName name)
    {
        m_name = name;
    }

    public List<Tank> m_tanks { get; private set; }
    public eFactionName m_name { get; private set; }

    private void Start()
    {
        m_tanks = new List<Tank>();
    }

    public void addTank(Tank tank)
    {
        m_tanks.Add(tank);
    }
}
