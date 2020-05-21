using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Assertions;

public class Harvester : MonoBehaviour
{
    public int m_harvestedResources { get; private set; }
    public int m_maximumExtractableAmount { get; private set; }
    public float m_timeBetweenPathUpdates { get; private set; }
    public float m_distanceToHarvest { get; private set; }
    public float m_destinationOffSetHQ { get; private set; }
    public Building m_buildingToReturnResource { get; set; }
    public BoidSpawner m_boidSpawner { get; set; }
    public Boid m_targetBoid { get; set; }

    private void Awake()
    {
        m_harvestedResources = 0;
        m_maximumExtractableAmount = 3;
        m_timeBetweenPathUpdates = 0.2f;
        m_distanceToHarvest = 1.5f;
        m_destinationOffSetHQ = 1.0f;
    }

    private void OnDestroy()
    {
        releaseTargetBoid();
    }

    public void releaseTargetBoid()
    {
        if (m_targetBoid && m_boidSpawner)
        { 
            m_boidSpawner.releaseBoid(m_targetBoid);
            m_targetBoid = null;
        }
    }

    public int extractHarvestedResources()
    {
        int harvestedResources = m_harvestedResources;
        m_harvestedResources = 0;

        return harvestedResources;
    }

    public void incrementResource(out bool resourceLimitReached)
    {
        ++m_harvestedResources;
        if(m_harvestedResources >= m_maximumExtractableAmount)
        {
            resourceLimitReached = true;
        }
        else
        {
            resourceLimitReached = false;
        }
    }
}