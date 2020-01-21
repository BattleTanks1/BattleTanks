using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Tank> m_tanks { get; private set; }
    private int m_ID;
    public const int INVALID_ID = -1;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        m_tanks = new List<Tank>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Tank GetTank(int ID) 
    {
        foreach(Tank tank in m_tanks)
        {
            if(tank.m_ID == ID)
            {
                return tank;
            }
        }

        return null;
    }

    public int addTank(Tank tank)
    {
        m_tanks.Add(tank);
        int ID = m_ID;
        ++m_ID;
        return ID;
    }
}
