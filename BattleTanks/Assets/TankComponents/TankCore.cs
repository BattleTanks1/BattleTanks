using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    player,
    AI
}

public class TankCore : MonoBehaviour
{
    [SerializeField]
    public int m_ID { get; protected set; }

    [SerializeField]
    public Faction m_faction;

    [SerializeField]
    protected int m_health;

    // Start is called before the first frame update
    void Start()
    {
        m_ID = GameManager.Instance.addTank(this);
    }

    void Update()
    {
        
    }

    public void damage(int amount)
    {
        m_health -= amount;
        //Death functionality
    }
}