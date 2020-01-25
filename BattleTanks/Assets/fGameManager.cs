﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fGameManager : MonoBehaviour
{
    public List<Tank> m_Tanks { get; private set; }
    private int m_ID = 0;

    public PlayerTank m_player;

    private static fGameManager _instance;
    public static fGameManager Instance { get { return _instance; } }

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

        m_Tanks = new List<Tank>();
    }

    public Tank GetTank(int ID)
    {
        foreach (Tank tank in m_Tanks)
        {
            if (tank.m_ID == ID)
            {
                return tank;
            }
        }

        return null;
    }

    public List<Tank> getAllTanks()
    {
        return m_Tanks;
    }

    public Tank GetTank(Faction faction)
    {
        foreach (Tank tank in m_Tanks)
        {
            if (tank.m_faction != faction)
            {
                return tank;
            }
        }

        return null;
    }

    public int addTank(Tank tank)
    {
        m_Tanks.Add(tank);
        int ID = m_ID;
        ++m_ID;
        return ID;
    }
}