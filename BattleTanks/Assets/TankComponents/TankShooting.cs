using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [SerializeField]
    private float m_shootRange;
    [SerializeField]
    private Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;
    [SerializeField]
    private float m_timeBetweenShot;
    
    private float m_elaspedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_elaspedTime += Time.deltaTime;
    }

    public void FireAtPosition(Vector3 position)
    {
        //If enemy in range
        if (m_elaspedTime >= m_timeBetweenShot &&
            Vector3.Distance(position, transform.position) <= m_shootRange)
        {
            m_elaspedTime = 0.0f;

            Rigidbody clone;
            clone = Instantiate(m_projectile, transform.position, Quaternion.identity);
            Vector3 vBetween = position - transform.position;
            clone.velocity = transform.TransformDirection(vBetween.normalized * m_projectileSpeed);
        }
    }

    public bool isTargetInAttackRange(Vector3 targetPosition)
    {
        Vector3 result = targetPosition - transform.position;
        return result.sqrMagnitude <= m_shootRange * m_shootRange;
    }
}