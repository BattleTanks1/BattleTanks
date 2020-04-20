﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Building : MonoBehaviour
{
    [SerializeField]
    private GameObject m_spawnableUnit = null;
    [SerializeField]
    private GameObject m_wayPointPrefab = null;

    private GameObject m_wayPointClone = null;
    private Selection m_selectionComponent = null;

    private void Awake()
    {
        Assert.IsNotNull(m_spawnableUnit);
        Assert.IsNotNull(m_wayPointPrefab);

        m_selectionComponent = GetComponent<Selection>();
        Assert.IsNotNull(m_selectionComponent);

        m_wayPointClone = Instantiate(m_wayPointPrefab, transform.position, Quaternion.identity);
    }

    public void setWayPoint(Vector3 position)
    {
        if(m_selectionComponent.contains(position))
        {
            //Reset waypoint
            m_wayPointClone.transform.position = transform.position;
            m_wayPointClone.SetActive(false);
           
        }
        else
        {
            //Assign waypoint to new position
            m_wayPointClone.SetActive(true);
            m_wayPointClone.transform.position = new Vector3(position.x, 1, position.z);
        }
    }

    public void showWayPoint()
    {
        m_wayPointClone.SetActive(true);
    }

    public void hideWayPoint()
    {
        m_wayPointClone.SetActive(false);
    }

    public GameObject spawnUnit()
    {
        Vector3 startingPosition = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f));
        int distance = 1;
        GameObject newTank = null;

        while(!newTank)
        {
            Vector3 spawnPosition = transform.position + startingPosition.normalized * distance;
            spawnPosition = new Vector3(spawnPosition.x, 1, spawnPosition.z);
            if(!m_selectionComponent.contains(spawnPosition) && !Map.Instance.isPositionOccupied(spawnPosition, Utilities.INVALID_ID))
            {
                newTank = Instantiate(m_spawnableUnit, spawnPosition, Quaternion.identity);
                
                //Move new tank to waypoint
                if(m_wayPointClone.transform.position != transform.position)
                {
                    Assert.IsTrue(m_wayPointClone.activeSelf);
                    TankPlayer tankPlayer = newTank.GetComponent<TankPlayer>();
                    Assert.IsNotNull(tankPlayer);
                    
                    tankPlayer.receiveMessage(new MessageToAIUnit(
                        Utilities.INVALID_ID, eAIState.MovingToNewPosition, Utilities.convertToGridPosition(m_wayPointClone.transform.position)));
                }
            }

            ++distance;
        }

        return newTank;
    }
}