using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tank : MonoBehaviour
{
    public float m_safePositionValue;
    public int m_threat;
    public float m_speed;
    public float m_strength;
    public int m_proximity;

    [SerializeField]
    public int m_ID { get; protected set; }

    [SerializeField]
    public eFactionName m_factionName { get; protected set; }

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


    protected virtual void Update()
    {
        m_shotTimer.update(Time.deltaTime);
        move(Time.deltaTime);
    }
}