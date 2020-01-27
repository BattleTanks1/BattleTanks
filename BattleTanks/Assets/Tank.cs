using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    player,
    AI
}

public class Tank : MonoBehaviour
{
    public float m_speed;
    public float m_strength;
    public int m_proximity;

    [SerializeField]
    public int m_ID { get; protected set; }

    [SerializeField]
    public Faction m_faction { get; protected set; }

    [SerializeField]
    private GameObject m_projectileSpawn = null;

    //Movement related variables
    [SerializeField]
    protected float m_movementSpeed;

    [SerializeField]
    protected float m_maxSpeed = 20;

    [SerializeField]
    protected float m_rotationSpeed;

    [SerializeField]
    protected float m_maxRotation = 50;

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
        m_minDistance = 3;
        m_shotTimer.m_active = true;
        m_shotTimer.m_expiredTime = 2.0f;
    }

    protected void move(float dTime)
    {
        transform.Rotate(new Vector3(0,m_rotationSpeed * Time.deltaTime, 0));
        transform.Translate(transform.forward * m_movementSpeed * dTime);
        m_movementSpeed = 0;
        m_rotationSpeed = 0;
    }

    protected void forward(float dTime)
    {
        m_movementSpeed += m_maxSpeed;
    }

    protected void backward(float dTime)
    {
        m_movementSpeed += -m_maxSpeed;
    }

    protected void leftTurn(float dTime)
    {
        m_rotationSpeed += m_maxRotation;
    }

    protected void rightTurn(float dTime)
    {
        m_rotationSpeed += -m_maxRotation;
    }

    protected virtual void Update()
    {
        m_shotTimer.update(Time.deltaTime);
        move(Time.deltaTime);
    }

    public void damage(int amount)
    {
        m_health -= amount;
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