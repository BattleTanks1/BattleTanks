using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eHarvesterState
{
    NotHarvesting = 0,
    SetDestinationHarvest,
    SetDestinationResourceBuilding,
    MovingToHarvestPosition,
    MovingToResourceBuilding,
    Harvesting,
    ReturningHarvestedResource
}

public class HarvesterStateHandler : UnitStateHandler
{
    [SerializeField]
    private float m_destinationOffSetResource = 1.0f;
    [SerializeField]
    private float m_destinationOffSetHQ = 1.0f;
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
                        switchToState(eHarvesterState.Harvesting);
                    }
                }   
                break;
            case eHarvesterState.Harvesting:
                {
                    Assert.IsNotNull(m_resourceToHarvest);
                    bool maximumExtracted = false;
                    if(m_harvester.extractResource(m_resourceToHarvest, out maximumExtracted) && maximumExtracted)
                    {
                        switchToState(eHarvesterState.SetDestinationResourceBuilding);
                    }
                }
                break;
            case eHarvesterState.MovingToResourceBuilding:
                {
                    if (m_tankMovement.reachedDestination() && m_resourceToHarvest)
                    {
                        GameManager.Instance.addResourcesToFaction(m_harvester);
                        switchToState(eHarvesterState.SetDestinationHarvest);
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
            case eHarvesterState.SetDestinationHarvest:
                {
                    if (resource)
                    {
                        m_resourceToHarvest = resource;
                    }

                    Assert.IsNotNull(m_resourceToHarvest);
                    fRectangle AABB = m_resourceToHarvest.GetComponent<Selection>().getAABB();
                    m_tankMovement.moveTo(
                        Utilities.getClosestPositionOutsideAABB(AABB, transform.position, m_resourceToHarvest.transform.position, m_destinationOffSetResource));
                    
                    m_harvesterState = eHarvesterState.MovingToHarvestPosition;
                }
                break;
            case eHarvesterState.SetDestinationResourceBuilding:
                {
                    Assert.IsNotNull(m_harvester.getBuildingToReturn());
                    
                    fRectangle AABB = m_harvester.getBuildingToReturn().GetComponent<Selection>().getAABB();
                    m_tankMovement.moveTo(
                        Utilities.getClosestPositionOutsideAABB(AABB, transform.position, m_harvester.getBuildingToReturn().transform.position, m_destinationOffSetHQ));
                    
                    m_harvesterState = eHarvesterState.MovingToResourceBuilding;
                }
                break;
        }
    }
}