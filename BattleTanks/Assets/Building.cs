using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class Building : MonoBehaviour
{
    [SerializeField]
    private float m_spawnOffSet = 1.0f;
    [SerializeField]
    private Unit m_tankToSpawn = null;
    [SerializeField]
    private Unit m_harvesterToSpawn = null;
    [SerializeField]
    private GameObject m_wayPointPrefab = null;
    [SerializeField]
    private BoidSpawner m_boidSpawner = null;

    private GameObject m_wayPointClone = null;
    private Selection m_selectionComponent = null;

    private void Awake()
    {
        Assert.IsNotNull(m_tankToSpawn);
        Assert.IsNotNull(m_harvesterToSpawn);
        Assert.IsNotNull(m_wayPointPrefab);
        Assert.IsNotNull(m_boidSpawner);

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
           
        }
        else if(Map.Instance.isInBounds(position))
        {
            //Assign waypoint to new position
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

    public Unit spawnUnit(eUnitType unitType)
    {
        Vector3 spawnPosition;
        if (m_wayPointClone.transform.position != transform.position)
        {
            spawnPosition = Utilities.getClosestPositionOutsideAABB(m_selectionComponent.getAABB(),
                m_wayPointClone.transform.position, transform.position, m_spawnOffSet);
        }
        else
        {
            spawnPosition = Utilities.getRandomPositionOutsideAABB(m_selectionComponent.getAABB(), 
                transform.position, m_spawnOffSet);
        }
        
        if (!Map.Instance.isPositionOccupied(spawnPosition))
        {
            Unit newUnit = null;
            if (unitType == eUnitType.Harvester)
            {
                newUnit = Instantiate(m_harvesterToSpawn, spawnPosition, Quaternion.identity);
                Harvester harvester = newUnit.GetComponent<Harvester>();
                Assert.IsNotNull(harvester);
                harvester.setBuildingToReturnResource(this);
            }
            else if(unitType == eUnitType.Attacker)
            {
                newUnit = Instantiate(m_tankToSpawn, spawnPosition, Quaternion.identity);
            }

            if (m_wayPointClone.transform.position != transform.position)
            {
                Assert.IsTrue(m_wayPointClone.activeSelf);
                UnitStateHandler stateHandlerComponent = newUnit.GetComponent<UnitStateHandler>();
                Assert.IsNotNull(stateHandlerComponent);

                stateHandlerComponent.switchToState(eUnitState.SetDestination, Utilities.INVALID_ID, m_wayPointClone.transform.position);
            }

            return newUnit;
        }
        else
        {
            return null;
        }
    }
}