using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Vector3 m_spawnPosition;
    [SerializeField]
    GameObject m_boidTemplate;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        Debug.Log("Gooooood morning vietnam");
        m_spawnPosition = transform.position;
        m_boids = new BoidTracker[m_maxActiveBoids];
        m_boidStockpile = m_maxBoids;
        for (int i = 0; i < m_boids.Length; ++i)
        {
            m_boids[i].m_deathTime = Time.time + 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                m_boids[i].m_boid.m_homePos = m_spawnPosition;
                m_boids[i].m_boid.setParent(this, i);
                m_boids[i].m_deathTime = 0.0f;
                --m_boidStockpile;
            }
        }
    }
}
