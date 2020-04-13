using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public float m_shootRange;
    [SerializeField]
    protected Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;

    public float m_timeBetweenShot;
    private float m_elaspedTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
