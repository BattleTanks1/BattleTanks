using System.Collections;
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

        if (m_currentState != eUnitState.InUseBySecondaryState)
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
        m_harvesterState = newState;
        m_currentState = eUnitState.InUseBySecondaryState;
        m_targetID = Utilities.INVALID_ID;

        switch (newState)
        {
            case eHarvesterState.MovingToHarvestPosition:
                {
                    if (resource)
                    {
                        m_resourceToHarvest = resource;
                    }

                    m_tankMovement.moveTo(getHarvestingPosition());
                }
                break;
            case eHarvesterState.MovingToResourceBuilding:
                {
                    Assert.IsNotNull(m_harvester.getBuildingToReturnResource());
                    m_tankMovement.moveTo(getReturnPosition(m_harvester.getBuildingToReturnResource()));
                }
                break;
        }
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

    private Vector3 getHarvestingPosition()
    {
        Assert.IsNotNull(m_resourceToHarvest);
        Selection resoureceSelection = m_resourceToHarvest.GetComponent<Selection>();
        Assert.IsNotNull(resoureceSelection);

        int distance = 1;
        Vector3 position = Utilities.INVALID_POSITION;
        do
        {
            position = m_resourceToHarvest.transform.position + (transform.position - m_resourceToHarvest.transform.position).normalized * distance;
            ++distance;

        } while (resoureceSelection.contains(position));

        return position;
    }
}