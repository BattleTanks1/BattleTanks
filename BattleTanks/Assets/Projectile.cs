using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int m_parentID = 0;
    [SerializeField]
    private float m_lifeTime = 0.0f;
    [SerializeField]
    private int m_damage = 0;

    private void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankCore tank = other.gameObject.GetComponent<TankCore>();
        if (tank && tank.m_ID != m_parentID)
        {
            tank.damage(m_damage);
            Destroy(gameObject);
        }
    }
}