using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HarvesterStateHandler : UnitStateHandler
{
    private Harvester m_harvester = null;
    private Resource m_resourceToHarvest = null;

    protected override void Awake()
    {
        base.Awake();

        m_harvester = GetComponent<Harvester>();
        Assert.IsNotNull(m_harvester);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        switch (m_currentState)
        {
            case eTankState.MovingToHarvestPosition:
                {
                    if(m_tankMovement.reachedDestination())
                    {
                        m_currentState = eTankState.Harvest;
                    }
                }   
                break;
            case eTankState.Harvest:
                {
                    Assert.IsNotNull(m_resourceToHarvest);

                    if(m_harvester.extractResource(m_resourceToHarvest))
                    {
                        m_currentState = eTankState.ReturnHarvestedResource;
                    }
                }
                break;

            case eTankState.ReturnHarvestedResource:
                {
                    Building buildingToReturnResource = m_harvester.getBuildingToReturnResource();
                    if(buildingToReturnResource)
                    {
                        m_tankMovement.moveTo(getReturnPosition(buildingToReturnResource));
                        m_currentState = eTankState.ReturningHarvestedResource;
                    }
                }
                break;
            case eTankState.ReturningHarvestedResource:
                {

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

        m_currentState = eTankState.MovingToHarvestPosition;
        m_resourceToHarvest = resourceToHarvest;
        switchToState(eTankState.MovingToHarvestPosition, Utilities.INVALID_ID, positionToMoveTo);
    }

    public override void switchToState(eTankState state, int targetID, Vector3 position)
    {
        base.switchToState(state, targetID, position);

        switch(state)
        {
            case eTankState.MovingToHarvestPosition:
                {
                    m_targetID = targetID;
                    m_currentState = state;
                    m_tankMovement.moveTo(position);
                }
                break;
        }
    }
}