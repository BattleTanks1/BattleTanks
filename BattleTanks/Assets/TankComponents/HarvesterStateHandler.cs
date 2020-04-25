using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HarvesterStateHandler : UnitStateHandler
{
    private Selection m_selection = null;
    private Resource m_resourceToHarvest = null;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(m_tankMovement);

        m_selection = GetComponent<Selection>();
        Assert.IsNotNull(m_selection);
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

                }
                break;
        }
    }

    private Vector3 getHarvestingPosition(Resource resource)
    {
        Assert.IsNotNull(resource);

        int distance = 1;
        Vector3 position = Utilities.INVALID_POSITION;
        do
        {
            position = transform.position + (resource.transform.position - transform.position).normalized * distance;
            ++distance;

        } while (!m_selection.contains(position));


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
            case eTankState.Harvest:
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