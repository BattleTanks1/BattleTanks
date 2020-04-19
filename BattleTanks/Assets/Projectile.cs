using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour
{
    private int m_senderID = Utilities.INVALID_ID;
    [SerializeField]
    private float m_lifeTime = 0.0f;
    [SerializeField]
    private int m_damage = 0;

    private void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    public void setSenderID(int senderID)
    {
        Assert.IsTrue(m_senderID == Utilities.INVALID_ID);
        m_senderID = senderID;
    }

    private void OnTriggerEnter(Collider other)
    {
        Tank tank = other.gameObject.GetComponent<Tank>();
        if (tank && tank.m_ID != m_senderID)
        {
            Destroy(gameObject);
        }
    }
}