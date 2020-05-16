using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoidTracker
{
    public Boid m_boid;
    public DateTime m_deathTime;
}

public class NewBehaviourScript : MonoBehaviour
{
    int m_maxBoids = 20;
    int m_maxActiveBoids = 10;
    BoidTracker[] m_boids;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        m_boids = BoidTracker[m_maxActiveBoids];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
