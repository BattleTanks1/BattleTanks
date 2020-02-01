using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public float m_safePositionValue;
    public float m_movementSpeed;
    public int m_visibilityDistance;

    public float m_threatStrength;
    public int m_threatDistance;

    public float m_proximityStrength;
    public int m_proximityDistance;

    public Vector3 m_oldPosition { get; protected set; }

    [SerializeField]
    public int m_ID { get; protected set; }

    [SerializeField]
    public eFactionName m_factionName;

    //Turret
    private Timer m_shotTimer;

    [SerializeField]
    private Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;

    private void Awake()
    {
        m_shotTimer = new Timer();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_shotTimer.m_active = true;
        m_shotTimer.m_expiredTime = 2.0f;

        m_oldPosition = transform.position;
        fGameManager.Instance.updatePositionOnMap(this);
    }

    protected virtual void Update()
    {
        m_shotTimer.update(Time.deltaTime);
    }
}