using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<AITank> m_tanks { get; private set; }
    public PlayerTank m_player { get; private set; }
    private int m_ID;

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

        m_tanks = new List<AITank>();
    }

    public AITank GetTank(int ID)
    {
        foreach (AITank tank in m_tanks)
        {
            if (tank.m_ID == ID)
            {
                return tank;
            }
        }

        return null;
    }

    public int addTank(AITank tank)
    {
        m_tanks.Add(tank);
        int ID = m_ID;
        ++m_ID;
        return ID;
    }

    public void addPlayerTank(PlayerTank tank)
    {
        m_player = tank;
    }
}