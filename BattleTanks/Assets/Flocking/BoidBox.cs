using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public struct BoidTracker
{
    public Boid m_boid;
    public float m_deathTime;
}

public class BoidBox : MonoBehaviour
{
    int m_maxBoids = 20;
    int m_boidStockpile;
    int m_maxActiveBoids = 10;
    BoidTracker[] m_boids;
    float m_respawnTime = 0.5f;
    [SerializeField]
    float m_spawnRate = 2.0f;
    Vector3 m_spawnPosition;
    [SerializeField]
    bool m_isTesting = false;
    [SerializeField]
    GameObject m_boidTemplate;

    //Boid defaults
    [SerializeField]
    private float m_boidBounds = 10.0f;
    [SerializeField]
    private float m_boidMaxAcceleration = 10.0f;
    [SerializeField]
    private float m_boidDragEffect = 0.0f;
    [SerializeField]
    private float m_boidAvoidanceDistance = 5.0f;
    [SerializeField]
    private float m_boidDetectionDistance = 5.0f;
    [SerializeField]
    private float m_boidViewAngle = 0.75f;
    // Start is called before the first frame update


    void Awake()
    {
        Debug.Log("Gooooood morning vietnam");
        m_spawnPosition = new Vector3(transform.position.x, 1.0f, transform.position.z);
        m_boids = new BoidTracker[m_maxActiveBoids];
        m_boidStockpile = m_maxBoids;
        for (int i = 0; i < m_boids.Length; ++i)
        {
            m_boids[i].m_deathTime = Time.time + 0.1f;
        }
    }
    void Start()
    {
        IEnumerator coroutine = spawnNewBoids();
        StartCoroutine(coroutine);
    }

    private IEnumerator spawnNewBoids()
    {
        while (true)
        {
            if (m_spawnRate != 0.0f)
            {
                yield return new WaitForSeconds(m_spawnRate);

                for (int i = 0; i < m_boids.Length; ++i)
                {
                    Debug.Log("Checking element" + i.ToString());
                    if (m_boids[i].m_deathTime != 0.0f && Time.time - m_boids[i].m_deathTime > m_respawnTime && m_boidStockpile > 0)
                    {
                        Debug.Log("HOOHAA");
                        //create a new wobject
                        GameObject newBoid = Instantiate(m_boidTemplate, m_spawnPosition, Quaternion.identity);
                        //Get the wobjects boid script via GetComponent<Boid>()
                        m_boids[i].m_boid = newBoid.GetComponent<Boid>();
                        //Set its home pos to your location
                        m_boids[i].m_boid.setParent(this, i);
                        m_boids[i].m_boid.setStats(m_spawnPosition, m_boidBounds, m_boidMaxAcceleration, m_boidDragEffect, m_boidAvoidanceDistance, m_boidDetectionDistance, m_boidViewAngle);
                        m_boids[i].m_deathTime = 0.0f;
                        --m_boidStockpile;
                        break;
                    }
                }
                //Keep updating stats if currently testing
                if (m_isTesting)
                {
                    for (int i = 0; i < m_boids.Length; ++i)
                    {
                        if (m_boids[i].m_boid == null)
                            continue;
                        m_boids[i].m_boid.setStats(m_spawnPosition, m_boidBounds, m_boidMaxAcceleration, m_boidDragEffect, m_boidAvoidanceDistance, m_boidDetectionDistance, m_boidViewAngle);
                    }
                }
            }
            else
                yield return new WaitForSeconds(1.0f);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    public BoidTracker[] getBoids()
    {
        return m_boids;
    }
}
