using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eHarvesterState
{
    NotHarvesting = 0,
    SetDestinationResourceBuilding,
    MovingToResourceBuilding,
    Initialize,
    TargetAvailableBoid,
    MovingToTargetedBoid,
    HarvestTargetedBoid
}

public class HarvesterStateHandler : UnitStateHandler
{
    [SerializeField]
    private eHarvesterState m_harvesterState;
    private Harvester m_harvester = null;
    private bool m_findAvailableBoid = false;

    protected override void Awake()
    {
        base.Awake();
        m_harvesterState = eHarvesterState.NotHarvesting;

        Harvester harvesterComponent = GetComponent<Harvester>();
        Assert.IsNotNull(harvesterComponent);
        m_harvester = harvesterComponent;
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(updateMovementPath());
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_currentState != eUnitState.InUseBySecondaryState)
        {
            m_harvesterState = eHarvesterState.NotHarvesting;
            m_harvester.releaseTargetBoid();
        }
        if(m_findAvailableBoid)
        {
            switchToState(eHarvesterState.TargetAvailableBoid);
        }

        switch (m_harvesterState)
        {
            case eHarvesterState.MovingToTargetedBoid:
                {
                    Assert.IsNotNull(m_harvester.m_targetBoid);
                    if ((m_harvester.m_targetBoid.transform.position - transform.position).sqrMagnitude <= 
                        m_harvester.m_distanceToHarvest * m_harvester.m_distanceToHarvest)
                    {
                        switchToState(eHarvesterState.HarvestTargetedBoid);
                    }
                }
                break;
            case eHarvesterState.HarvestTargetedBoid:
                {
                    Assert.IsNotNull(m_harvester.m_targetBoid);
                    if ((m_harvester.m_targetBoid.transform.position - transform.position).sqrMagnitude <= 
                        m_harvester.m_distanceToHarvest * m_harvester.m_distanceToHarvest)
                    {
                        Assert.IsNotNull(m_harvester.m_boidSpawner);
                        m_harvester.m_boidSpawner.destroyBoid(m_harvester.m_targetBoid);
                        
                        bool harvesterResourceCapacityReached = false;
                        m_harvester.incrementResource(out harvesterResourceCapacityReached);
                        if (harvesterResourceCapacityReached)
                        {
                            switchToState(eHarvesterState.SetDestinationResourceBuilding);
                        }
                        else
                        {
                            switchToState(eHarvesterState.TargetAvailableBoid);
                        }
                    }
                }
                break;
            case eHarvesterState.MovingToResourceBuilding:
                {
                    if (m_tankMovement.reachedDestination())
                    {
                        m_harvester.extractHarvestedResources();
                        GameManager.Instance.addResourcesToFaction(m_harvester);
                        switchToState(eHarvesterState.TargetAvailableBoid);
                    }
                }
                break;
        }
    }

    private IEnumerator updateMovementPath()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(m_harvester.m_timeBetweenPathUpdates);

            if (m_harvester.m_targetBoid)
            {
                m_tankMovement.moveTo(m_harvester.m_targetBoid.transform.position);
            }
        }
    }

    public void switchToState(eHarvesterState newState, BoidSpawner boidSpawner = null)
    {
        m_harvesterState = newState;
        m_currentState = eUnitState.InUseBySecondaryState;
        m_targetID = Utilities.INVALID_ID;

        switch (newState)
        {
            case eHarvesterState.Initialize:
                {
                    Assert.IsNotNull(boidSpawner);

                    m_harvester.m_boidSpawner = boidSpawner;
                }
                break;
            case eHarvesterState.TargetAvailableBoid:
                {
                    Assert.IsNotNull(m_harvester.m_boidSpawner);

                    m_harvester.m_targetBoid = m_harvester.m_boidSpawner.getAvailableBoid(m_unit.getID());
                    if(m_harvester.m_targetBoid)
                    {
                        m_tankMovement.moveTo(m_harvester.m_targetBoid.transform.position);
                        m_harvesterState = eHarvesterState.MovingToTargetedBoid;
                        m_findAvailableBoid = false;
                    }
                    else
                    {
                        m_findAvailableBoid = true;
                    }
                }
                break;
            case eHarvesterState.SetDestinationResourceBuilding:
                {
                    Assert.IsNotNull(m_harvester.m_buildingToReturnResource);
                    
                    fRectangle AABB = m_harvester.m_buildingToReturnResource.GetComponent<Selection>().getAABB();
                    m_tankMovement.moveTo(
                        Utilities.getClosestPositionOutsideAABB(AABB, transform.position, 
                        m_harvester.m_buildingToReturnResource.transform.position, m_harvester.m_destinationOffSetHQ));
                    
                    m_harvesterState = eHarvesterState.MovingToResourceBuilding;
                }
                break;
        }
    }
}