using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Harvester : MonoBehaviour
{
    [SerializeField]
    private int m_extractAmount = 0;
    [SerializeField]
    private int m_maxExtractAmount = 0;
    [SerializeField]
    private float m_timeBetweenExtract = 0.0f;
    [SerializeField]
    private Building m_buildingToReturnResource = null;

    private int m_currentResourcesExtracted = 0;
    private float m_elaspedTime = 0.0f;

    private void Awake()
    {
        Assert.IsNotNull(m_buildingToReturnResource);
    }

    // Update is called once per frame
    private void Update()
    {
        m_elaspedTime += Time.deltaTime;
    }

    public bool extractResource(Resource resourceToHarvest)
    {
        if(m_elaspedTime >= m_timeBetweenExtract)
        {
            resourceToHarvest.extractResource(m_extractAmount);
            m_elaspedTime = 0.0f;

            return true;
        }
        else
        {
            return false;
        }
    }

    public Building getBuildingToReturnResource()
    {
        Assert.IsNotNull(m_buildingToReturnResource);

        return m_buildingToReturnResource;
    }
}