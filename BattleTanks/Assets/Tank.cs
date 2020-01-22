using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public int m_ID { get; protected set; }

    [SerializeField]
    private GameObject m_projectileSpawn = null;
    [SerializeField]
    private GameObject m_projectile = null;

    [SerializeField]
    private float m_minDistance { get; set; }

    [SerializeField]
    protected int m_health;
    public int health { get { return m_health; } protected set { m_health = health; } }

    [SerializeField]
    protected Vector3 m_velocity;
    public Vector3 velocity { get { return m_velocity; } protected set { m_velocity = velocity; } }

    [SerializeField]
    protected float m_rotationSpeed;
    public float rotationSpeed { get { return m_rotationSpeed; } protected set { m_rotationSpeed = rotationSpeed; } }

    public float m_movementSpeed;
    private Timer m_shotTimer;

    private void Awake()
    {
        m_shotTimer = new Timer();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {  
        m_health = 5;
        m_minDistance = 3;
        m_shotTimer.m_active = true;
    }

    protected virtual void Update()
    {
        m_shotTimer.update(Time.deltaTime);
    }

    public void damage(int amount)
    {
        m_health -= amount;
    }

    protected void shoot()
    {
        if(m_shotTimer.isExpired())
        {
            GameObject projectile;
            projectile = Instantiate(m_projectile, m_projectileSpawn.transform.position, m_projectile.transform.rotation);

            m_shotTimer.reset();
        }
    }

    protected bool isInRange(Vector3 position)
    {
        return (Vector3.Distance(transform.position, position) <= Mathf.Abs(m_minDistance));
    }
}