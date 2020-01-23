﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    player,
    AI
}

public class Tank : MonoBehaviour
{
    [SerializeField]
    public int m_ID { get; protected set; }

    [SerializeField]
    public Faction m_faction { get; protected set; }

    [SerializeField]
    private GameObject m_projectileSpawn = null;


    [SerializeField]
    protected int m_health;

    //Turret
    private Timer m_shotTimer;

    [SerializeField]
    private Rigidbody m_projectile = null;

    [SerializeField]
    private float m_projectileSpeed = 0.0f;

    [SerializeField]
    protected float m_minDistance;

    private void Awake()
    {
        m_shotTimer = new Timer();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_ID = fGameManager.Instance.addTank(this);
        m_minDistance = 3;
        m_shotTimer.m_active = true;
        m_shotTimer.m_expiredTime = 2.0f;
    }

    protected virtual void Update()
    {
        m_shotTimer.update(Time.deltaTime);
    }

    public void damage(int amount)
    {
        m_health -= amount;
        //Death functionality
    }

    protected void shoot()
    {
        if(m_shotTimer.isExpired())
        {
            Rigidbody projectile;
            projectile = Instantiate(m_projectile, m_projectileSpawn.transform.position, m_projectile.transform.rotation);
            projectile.gameObject.GetComponent<Projectile>().m_parentID = m_ID;
            projectile.AddForce(transform.TransformDirection(Vector3.forward * m_projectileSpeed));

            m_shotTimer.reset();
        }
    }

    protected bool isInRange(Vector3 position)
    {
        bool inRange = false;
        if(Vector3.Distance(transform.position, position) <= Mathf.Abs(m_minDistance))
        {
            Vector3 vBetween = position - transform.position;
            if (Vector3.Dot(Vector3.forward, vBetween.normalized) <= -0.5f)
            {
                inRange = true;
            }
        }

        return inRange;
    }
}