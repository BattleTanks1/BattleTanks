﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eHarvesterState
{
    NotHarvesting = 0,
    MovingToHarvestPosition,
    MovingToResourceBuilding,
    Harvest,
    ReturningHarvestedResource
}

public class HarvesterStateHandler : UnitStateHandler
{
    [SerializeField]
    private eHarvesterState m_harvesterState;
    private Harvester m_harvester = null;
    private Resource m_resourceToHarvest = null;

    protected override void Awake()
    {
        base.Awake();
        m_harvesterState = eHarvesterState.NotHarvesting;

        Harvester harvesterComponent = GetComponent<Harvester>();
        Assert.IsNotNull(harvesterComponent);
        m_harvester = harvesterComponent;
    }

    protected override void Update()
    {
        base.Update();

        //Assert.IsTrue(m_currentState != eUnitState.AwaitingDecision && m_harvesterState == eHarvesterState.NotHarvesting);
        if(m_currentState != eUnitState.AwaitingDecision)
        {
            m_harvesterState = eHarvesterState.NotHarvesting;
        }

        switch (m_harvesterState)
        {
            case eHarvesterState.MovingToHarvestPosition:
                {
                    if(m_tankMovement.reachedDestination())
                    {
                        switchToState(eHarvesterState.Harvest);
                    }
                }   
                break;
            case eHarvesterState.Harvest:
                {
                    if(m_harvester.extractResource(m_resourceToHarvest))
                    {
                        switchToState(eHarvesterState.MovingToResourceBuilding);
                    }
                }
                break;
            case eHarvesterState.MovingToResourceBuilding:
                {
                    if (m_tankMovement.reachedDestination() && m_resourceToHarvest)
                    {
                        switchToState(eHarvesterState.MovingToHarvestPosition);
                    }
                }
                break;
        }
    }

    public void switchToState(eHarvesterState newState, Resource resource = null)
    {
        switch (newState)
        {
            case eHarvesterState.MovingToHarvestPosition:
                {
                    if (resource)
                    {
                        m_resourceToHarvest = resource;
                    }

                    m_tankMovement.moveTo(getHarvestingPosition(m_resourceToHarvest));
                }
                break;
            case eHarvesterState.MovingToResourceBuilding:
                {
                    Building buildingToReturnResource = m_harvester.getBuildingToReturnResource();
                    if (buildingToReturnResource)
                    {
                        m_tankMovement.moveTo(getReturnPosition(buildingToReturnResource));
                    }
                }
                break;
        }

        m_harvesterState = newState;
        m_targetID = Utilities.INVALID_ID;
    }

    private Vector3 getReturnPosition(Building building)
    {
        Assert.IsNotNull(building);
        Selection resoureceSelection = building.GetComponent<Selection>();
        Assert.IsNotNull(resoureceSelection);

        int distance = 1;
        Vector3 position = Utilities.INVALID_POSITION;
        do
        {
            position = building.transform.position + (transform.position - building.transform.position).normalized * distance;
            ++distance;

        } while (resoureceSelection.contains(position));

        return position;
    }

    private Vector3 getHarvestingPosition(Resource resource)
    {
        Assert.IsNotNull(resource);
        Selection resoureceSelection = resource.GetComponent<Selection>();
        Assert.IsNotNull(resoureceSelection);

        int distance = 1;
        Vector3 position = Utilities.INVALID_POSITION;
        do
        {
            position = resource.transform.position + (transform.position - resource.transform.position).normalized * distance;
            ++distance;

        } while (resoureceSelection.contains(position));

        return position;
    }
}