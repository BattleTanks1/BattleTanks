using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float m_lifeTime = 0.0f;
    [SerializeField]
    private int m_damage = 0;

    private eFactionName m_senderFaction;
    private int m_senderID = Utilities.INVALID_ID;

    private void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    public void setSenderID(int senderID, eFactionName factionName)
    {
        Assert.IsTrue(m_senderID == Utilities.INVALID_ID);
        m_senderFaction = factionName;
        m_senderID = senderID;
    }

    private void OnTriggerEnter(Collider other)
    {
        Tank tank = other.gameObject.GetComponent<Tank>();
        Assert.IsNotNull(tank);

        if (tank.m_ID != m_senderID && m_senderFaction != tank.m_factionName)
        {
            GameManager.Instance.damageTank(tank, m_damage);
            Destroy(gameObject);
        }
    }
}