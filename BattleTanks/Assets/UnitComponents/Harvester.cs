using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Harvester : MonoBehaviour
{
    [SerializeField]
    private int m_harvestedResources = 0;
    [SerializeField]
    private int m_maximumExtractableAmount = 2;
    [SerializeField]
    private Building m_buildingToReturnResource = null;
    public BoidSpawner m_boidSpawner { get; set; }
    public Boid m_targetBoid { get; set; }

    public Building getBuildingToReturnResource()
    {
        Assert.IsNotNull(m_buildingToReturnResource);

        return m_buildingToReturnResource;
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

    public void setBuildingToReturnResource(Building building)
    {
        Assert.IsNotNull(building);

        m_buildingToReturnResource = building;
    }
}