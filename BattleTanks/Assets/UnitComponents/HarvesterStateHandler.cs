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
    SetBoidSpawner,
    TargetAvailableBoid,
    MovingToTargetedBoid,
    HarvestTargetedBoid
}

public class HarvesterStateHandler : UnitStateHandler
{
    [SerializeField]
    private float m_timeBetweenPathUpdates = 0.2f;
    [SerializeField]
    private float m_distanceToHarvest = 1.0f;
    [SerializeField]
    private float m_destinationOffSetHQ = 1.0f;
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
                    if ((m_harvester.m_targetBoid.transform.position - transform.position).sqrMagnitude <= m_distanceToHarvest * m_distanceToHarvest)
                    {
                        Debug.Log("Begin Harvest");
                        switchToState(eHarvesterState.HarvestTargetedBoid);
                    }
                    //else if(m_tankMovement.reachedDestination())
                    //{
                    //    Debug.Log("Reached Destination");
                    //}

                    //if (m_tankMovement.reachedDestination())
                    //{
                    //    switchToState(eHarvesterState.Harvesting);
                    //}
                    }
                    break;
            case eHarvesterState.HarvestTargetedBoid:
                {
                    //if(m_tankMovement.reachedDestination()) //From move towards
                    //{
                    //    Destroy(m_boidToHarvest.gameObject);
                    //    m_boidToHarvest = null;
                    //}
                    if ((m_harvester.m_targetBoid.transform.position - transform.position).sqrMagnitude <= m_distanceToHarvest * m_distanceToHarvest)
                    {
                        Assert.IsNotNull(m_harvester.m_targetBoid);
                        Assert.IsNotNull(m_harvester.m_boidSpawner);

                        m_harvester.m_boidSpawner.destroyBoid(m_harvester.m_targetBoid);
                        Debug.Log("Harvested Boid");
                        switchToState(eHarvesterState.TargetAvailableBoid);
                    }
                    else
                    {
                        //Resource lost
                        //Acquire new resource to harvest
                    }
                    //bool maximumExtracted = false;
                    //if(m_harvester.extractResource(m_boidToHarvest, out maximumExtracted) && maximumExtracted)
                    //{
                    //    switchToState(eHarvesterState.SetDestinationResourceBuilding);
                    //}
                }
                break;
            case eHarvesterState.MovingToResourceBuilding:
                {
                    if (m_tankMovement.reachedDestination())
                    {
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
            yield return new WaitForSeconds(m_timeBetweenPathUpdates);

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
            case eHarvesterState.SetBoidSpawner:
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
                    Assert.IsNotNull(m_harvester.getBuildingToReturnResource());
                    
                    fRectangle AABB = m_harvester.getBuildingToReturnResource().GetComponent<Selection>().getAABB();
                    m_tankMovement.moveTo(
                        Utilities.getClosestPositionOutsideAABB(AABB, transform.position, m_harvester.getBuildingToReturnResource().transform.position, m_destinationOffSetHQ));
                    
                    m_harvesterState = eHarvesterState.MovingToResourceBuilding;
                }
                break;
        }
    }
}