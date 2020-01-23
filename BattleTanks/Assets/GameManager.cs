using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<TankCore> m_Tanks { get; private set; }
    private int m_ID = 0;
   

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

        m_Tanks = new List<TankCore>();
    }

    public TankCore GetTank(int ID)
    {
        foreach (TankCore tank in m_Tanks)
        {
            if (tank.m_ID == ID)
            {
                return tank;
            }
        }

        return null;
    }

    public TankCore GetTank(Faction faction)
    {
        foreach (TankCore tank in m_Tanks)
        {
            if (tank.m_faction != faction)
            {
                return tank;
            }
        }

        return null;
    }

    public int addTank(TankCore tank)
    {
        m_Tanks.Add(tank);
        int ID = m_ID;
        ++m_ID;
        return ID;
    }
}