﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int m_parent_id = 0;
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
        Tank tank = other.gameObject.GetComponent<Tank>();
        if (tank && tank.m_ID != m_parent_id)
        {
            tank.damage(m_damage);
            Destroy(gameObject);
        }
    }
}