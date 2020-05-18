using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Harvester : MonoBehaviour
{
    [SerializeField]
    private int m_extractedResources = 0;
    [SerializeField]
    private int m_maximumExtractableAmount = 1;
    [SerializeField]
    private float m_timeBetweenExtract = 0.0f;
    [SerializeField]
    private Building m_buildingToReturnResource = null;

    private float m_elaspedTime = 0.0f;

    // Update is called once per frame
    private void Update()
    {
        m_elaspedTime += Time.deltaTime;
    }

    public bool extractResource(Resource resourceToHarvest, out bool maximumExtracted)
    {
        Assert.IsNotNull(resourceToHarvest);
        if(m_elaspedTime >= m_timeBetweenExtract)
        {
            m_elaspedTime = 0.0f;
            m_extractedResources += resourceToHarvest.extractResource();
            maximumExtracted = m_extractedResources >= m_maximumExtractableAmount;

            return true;
        }

        maximumExtracted = false;
        return false;
    }

    public Building getBuildingToReturnResource()
    {
        Assert.IsNotNull(m_buildingToReturnResource);

        return m_buildingToReturnResource;
    }

    public int extractResources()
    {
        int resources = m_extractedResources;
        m_extractedResources = 0;

        return resources;
    }

    public void setBuildingToReturnResource(Building building)
    {
        Assert.IsNotNull(building);

        m_buildingToReturnResource = building;
    }
}