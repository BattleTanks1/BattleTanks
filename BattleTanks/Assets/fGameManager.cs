using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fGameManager : MonoBehaviour
{
    public List<AITank> m_AItanks { get; private set; }
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

        m_AItanks = new List<AITank>();
    }

    public Tank GetTank(int ID)
    {
        foreach (Tank tank in m_AItanks)
        {
            if (tank.m_ID == ID)
            {
                return tank;
            }
        }

        return null;
    }

    public List<AITank> getAllAITanks()
    {
        return m_AItanks;
    }

    public Tank GetTank(Faction faction)
    {
        foreach (Tank tank in m_AItanks)
        {
            if (tank.m_faction != faction)
            {
                return tank;
            }
        }

        return null;
    }

    public int addTank(AITank tank)
    {
        m_AItanks.Add(tank);
        int ID = m_ID;
        ++m_ID;
        return ID;
    }
}