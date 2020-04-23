using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TankShooting : MonoBehaviour
{
    [SerializeField]
    private float m_shootRange = 0.0f;
    [SerializeField]
    private Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;
    [SerializeField]
    private float m_timeBetweenShot = 0.0f;
    
    private float m_elaspedTime = 0.0f;
    private Tank m_tank = null;

    private void Awake()
    {
        m_tank = GetComponent<Tank>();
        Assert.IsNotNull(m_tank);
    }

    void Update()
    {
        m_elaspedTime += Time.deltaTime;
    }

    public void FireAtPosition(Vector3 position)
    {
        //If enemy in range
        Vector3 result = position - transform.position;
        if (m_elaspedTime >= m_timeBetweenShot &&
            result.sqrMagnitude <= m_shootRange * m_shootRange)
        {
            m_elaspedTime = 0.0f;

            Rigidbody clone;
            clone = Instantiate(m_projectile, transform.position, Quaternion.identity);
            Vector3 vBetween = position - transform.position;
            clone.velocity = transform.TransformDirection(vBetween.normalized * m_projectileSpeed);

            Projectile projectile = clone.GetComponent<Projectile>();
            Assert.IsNotNull(projectile);
            projectile.setSenderID(m_tank.m_ID, m_tank.m_factionName);
        }
    }

    public bool isTargetInAttackRange(Vector3 targetPosition)
    {
        Vector3 result = targetPosition - transform.position;
        return result.sqrMagnitude <= m_shootRange * m_shootRange;
    }
}