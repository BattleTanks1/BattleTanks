using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject m_projectileSpawn = null;

    [SerializeField]
    private Rigidbody m_projectile = null;

    [SerializeField]
    private float m_projectileSpeed = 0.0f;

    [SerializeField]
    protected float m_minDistance;

    private Timer m_shotTimer;

    private void Awake()
    {
        m_shotTimer = new Timer();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_minDistance = 3;
        m_shotTimer.m_active = true;
        m_shotTimer.m_expiredTime = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_shotTimer.update(Time.deltaTime);
    }

    protected bool isInRange(Vector3 position) //TODO I don't think this does what it says it does
    {
        bool inRange = false;
        if (Vector3.Distance(transform.position, position) <= Mathf.Abs(m_minDistance))
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
