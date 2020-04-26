using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

enum eHarvesterState
{
    NotHarvesting = 0,
    MovingToHarvestPosition,
    Harvest,
    SetDestinationResourceBuilding,
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
        m_harvester = GetComponent<Harvester>();
        Assert.IsNotNull(m_harvester);
    }

    protected override void Update()
    {
        base.Update();

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
                        m_harvesterState = eHarvesterState.Harvest;
                    }
                }   
                break;
            case eHarvesterState.Harvest:
                {
                    Assert.IsNotNull(m_resourceToHarvest);

                    if(m_harvester.extractResource(m_resourceToHarvest))
                    {
                        m_harvesterState = eHarvesterState.SetDestinationResourceBuilding;
                    }
                }
                break;

            case eHarvesterState.SetDestinationResourceBuilding:
                {
                    Building buildingToReturnResource = m_harvester.getBuildingToReturnResource();
                    if(buildingToReturnResource)
                    {
                        m_tankMovement.moveTo(getReturnPosition(buildingToReturnResource));
                        m_harvesterState = eHarvesterState.ReturningHarvestedResource;
                    }
                }
                break;
            case eHarvesterState.ReturningHarvestedResource:
                {
                    if(m_tankMovement.reachedDestination() && m_resourceToHarvest)
                    {
                        harvest(m_resourceToHarvest);
                    }
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

    public void harvest(Resource resourceToHarvest)
    {
        Assert.IsNotNull(resourceToHarvest);

        Vector3 positionToMoveTo = getHarvestingPosition(resourceToHarvest);
        Assert.IsTrue(positionToMoveTo != Utilities.INVALID_POSITION);

        m_harvesterState = eHarvesterState.MovingToHarvestPosition;
        m_resourceToHarvest = resourceToHarvest;

        m_targetID = Utilities.INVALID_ID;
        m_harvesterState = eHarvesterState.MovingToHarvestPosition;
        m_tankMovement.moveTo(positionToMoveTo);
    }
}