using System.Collections;
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

    private Vector3 getSpawnPosition()
    {
        Vector3 startingPosition;
        if(m_wayPointClone.transform.position != transform.position)
        {
            startingPosition = (m_wayPointClone.transform.position - transform.position).normalized;
        }
        else
        {
            startingPosition = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f));
        }

        Vector3 spawnPosition;
        int distance = 1;
        do
        {
            spawnPosition = transform.position + startingPosition.normalized * distance;
            ++distance;
        }
        while (m_selectionComponent.contains(spawnPosition));

        return new Vector3(spawnPosition.x, 1, spawnPosition.z);
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

    public GameObject spawnUnit()
    {
        GameObject newTank = null;
        Vector3 spawnPosition = getSpawnPosition();
        
        if (!Map.Instance.isPositionOccupied(spawnPosition))
        {
            newTank = Instantiate(m_spawnableUnit, spawnPosition, Quaternion.identity);
            newTank.transform.parent = transform.parent;

            if (m_wayPointClone.transform.position != transform.position)
            {
                Assert.IsTrue(m_wayPointClone.activeSelf);
                UnitStateHandler stateHandlerComponent = newTank.GetComponent<UnitStateHandler>();
                Assert.IsNotNull(stateHandlerComponent);

                stateHandlerComponent.switchToState(eUnitState.MovingToNewPosition, Utilities.INVALID_ID, m_wayPointClone.transform.position);
            }

            return newTank;
        }
        else
        {
            return null;
        }
    }
}