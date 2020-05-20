using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Assertions;

public class Harvester : MonoBehaviour
{
    [SerializeField]
    private int m_harvestedResources = 0;
    [SerializeField]
    private int m_maximumExtractableAmount = 2;

    public Building m_buildingToReturnResource { get; set; }
    public BoidSpawner m_boidSpawner { get; set; }
    public Boid m_targetBoid { get; set; }

    public void releaseTargetBoid()
    {
        if (m_targetBoid)
        {
            Assert.IsNotNull(m_boidSpawner);
            m_boidSpawner.releaseBoid(m_targetBoid);
            m_targetBoid = null;
        }
    }

    private void OnDestroy()
    {
        releaseTargetBoid();
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