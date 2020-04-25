using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour
{
    [SerializeField]
    private int m_extractAmount = 0;
    [SerializeField]
    private int m_maxExtractAmount = 0;
    [SerializeField]
    private float m_timeBetweenExtract = 0.0f;

    private int m_currentResourcesExtracted = 0;
    private float m_elaspedTime = 0.0f;

    // Update is called once per frame
    void Update()
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
}