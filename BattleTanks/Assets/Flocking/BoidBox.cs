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
    float m_respawnTime;

    GameObject m_boidTemplate;



    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        m_boids = BoidTracker[m_maxActiveBoids];
        m_boidStockpile = m_maxBoids;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BoidTracker element in m_boids)
        {
            if (element.m_deathTime != 0.0f && Time.time - element.m_deathTime > m_respawnTime && m_boidStockpile > 0)
            {
                //create a new wobject
                GameObject newBoid = Instantiate(m_boidTemplate, transform.position, Quaternion.identity);
                //Get the wobjects boid script via GetComponent<Boid>()
                element.m_boid = newBoid.GetComponent<Boid>();
                //Set its home pos to your location
                element.m_boid.m_homePos = transform.position;
                element.m_boid.setParent(this);
                element.m_deathTime = 0.0f;
                m_boidStockpile--;
            }
        }
    }
}
