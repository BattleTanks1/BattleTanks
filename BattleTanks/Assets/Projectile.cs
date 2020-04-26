using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float m_lifeTime = 0.0f;

    private int m_damage = 0;
    private eFactionName m_senderFaction;
    private int m_senderID = Utilities.INVALID_ID;

    private void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    public void setSenderID(int senderID, eFactionName factionName, int damage)
    {
        Assert.IsTrue(m_senderID == Utilities.INVALID_ID);
        m_senderFaction = factionName;
        m_senderID = senderID;
        m_damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit && unit.getID() != m_senderID && m_senderFaction != unit.m_factionName)
        {
            GameManager.Instance.damageUnit(unit, m_damage);
            Destroy(gameObject);
        }
    }
}