using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public int m_ID { get; protected set; }

    [SerializeField]
    private GameObject m_projectileSpawn;
    [SerializeField]
    private GameObject m_projectile;

    [SerializeField]
    public float m_minDistance { get; protected set; }

    [SerializeField]
    protected int m_health;
    public int health { get { return m_health; } protected set { m_health = health; } }

    [SerializeField]
    protected Vector3 m_velocity;
    public Vector3 velocity { get { return m_velocity; } protected set { m_velocity = velocity; } }

    [SerializeField]
    protected float m_rotationSpeed;
    public float rotationSpeed { get { return m_rotationSpeed; } protected set { m_rotationSpeed = rotationSpeed; } }


    // Start is called before the first frame update
    void Start()
    {
        m_health = 5;
        m_minDistance = 3;
    }

    private void Update()
    {
        transform.position += m_velocity * Time.deltaTime;
    }

    public void damage(int amount)
    {
        m_health -= amount;
    }

    protected void shoot()
    {
        GameObject projectile;
        projectile = Instantiate(m_projectile, m_projectileSpawn.transform.position, m_projectile.transform.rotation);
    }
}
