using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BoidTracker
{ 
    public BoidTracker(float deathTime, int boidID)
    {
        m_boid = null;
        m_deathTime = deathTime;
        m_boidID = boidID;
    }

    public Boid m_boid;
    public float m_deathTime;
    public int m_boidID;
    public int m_harvesterID = Utilities.INVALID_ID;
}

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    private int m_maxBoids = 20;
    [SerializeField]
    private int m_boidsRemaining;
    [SerializeField]
    private int m_maxActiveBoids = 10;
    BoidTracker[] m_boids;
    private float m_respawnTime = 0.5f;
    [SerializeField]
    private float m_spawnRate = 2.0f;
    private Vector3 m_spawnPosition;
    [SerializeField]
    private bool m_isTesting = false;
    [SerializeField]
    private GameObject m_boidTemplate;

    //Boid defaults
    [SerializeField]
    private float m_boidBounds = 10.0f;
    [SerializeField]
    private float m_boidMaxAcceleration = 3.0f;
    [SerializeField]
    private float m_boidSpeed = 5.0f;
    [SerializeField]
    private float m_boidAvoidanceDistance = 3.0f;
    [SerializeField]
    private float m_boidDetectionDistance = 10.0f;
    [SerializeField]
    private float m_boidViewAngle = 0.75f;
    // Start is called before the first frame update

    void Awake()
    {
        m_spawnPosition = new Vector3(transform.position.x, 1.0f, transform.position.z);
        m_boids = new BoidTracker[m_maxActiveBoids];
        m_boidsRemaining = m_maxBoids;
        for (int i = 0; i < m_boids.Length; ++i)
        {
            m_boids[i] = new BoidTracker(Time.time + 0.1f, i);
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
                    //Debug.Log("Checking element" + i.ToString());
                    if (m_boids[i].m_deathTime != 0.0f && Time.time - m_boids[i].m_deathTime > m_respawnTime && m_boidsRemaining > 0)
                    {
                        //create a new wobject
                        GameObject newBoid = Instantiate(m_boidTemplate, m_spawnPosition, Quaternion.identity);
                        //Get the wobjects boid script via GetComponent<Boid>()
                        m_boids[i].m_boid = newBoid.GetComponent<Boid>();
                        //Set its home pos to your location
                        m_boids[i].m_boid.setParent(this, i);
                        m_boids[i].m_boid.setStats(m_spawnPosition, m_boidBounds, m_boidMaxAcceleration, m_boidSpeed, m_boidAvoidanceDistance, m_boidDetectionDistance, m_boidViewAngle);
                        m_boids[i].m_deathTime = 0.0f;
                        --m_boidsRemaining;
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
                        m_boids[i].m_boid.setStats(m_spawnPosition, m_boidBounds, m_boidMaxAcceleration, m_boidSpeed, m_boidAvoidanceDistance, m_boidDetectionDistance, m_boidViewAngle);
                    }
                }
            }
            else
                yield return new WaitForSeconds(1.0f);
        }
    }

    public void releaseBoid(Boid boidToRelease)
    {
        foreach (BoidTracker boid in m_boids)
        {
            if (boid.m_boid == boidToRelease)
            {
                boid.m_harvesterID = Utilities.INVALID_ID;
                return;
            }
        }

        Assert.IsTrue(false);
    }

    public void destroyBoid(Boid boidToDestroy)
    {
        foreach (BoidTracker boid in m_boids)
        {
            if (boid.m_boid == boidToDestroy)
            {
                Destroy(boid.m_boid.gameObject);
                return;
            }
        }

        Assert.IsTrue(false);
    }

    public Boid getAvailableBoid(int harvesterID)
    {
        foreach(BoidTracker boid in m_boids)
        {
            if(boid.m_harvesterID == Utilities.INVALID_ID && boid.m_deathTime == 0.0f)
            {
                boid.m_harvesterID = harvesterID;
                return boid.m_boid;
            }
        }

        return null;
    }

    public BoidTracker[] getBoids()
    {
        return m_boids;
    }

    public void killBoid(int index)
    {
        if (index >= 0 && index < m_boids.Length)
        {
            m_boids[index].m_boid = null;
            m_boids[index].m_deathTime = Time.time;
            m_boids[index].m_harvesterID = Utilities.INVALID_ID;
        }
    }
}