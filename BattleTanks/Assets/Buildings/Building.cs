using UnityEngine;
using UnityEngine.Assertions;

public class Building : MonoBehaviour
{
    [SerializeField]
    private float m_spawnOffSet = 0.0f;
    [SerializeField]
    private Unit m_tankToSpawn = null;
    [SerializeField]
    private Unit m_harvesterToSpawn = null;
    [SerializeField]
    private GameObject m_wayPointPrefab = null;

    private GameObject m_wayPointClone = null;
    private Selection m_selectionComponent = null;

    private void Awake()
    {
        Assert.IsNotNull(m_tankToSpawn);
        Assert.IsNotNull(m_harvesterToSpawn);
        Assert.IsNotNull(m_wayPointPrefab);

        m_selectionComponent = GetComponent<Selection>();
        Assert.IsNotNull(m_selectionComponent);

        m_wayPointClone = Instantiate(m_wayPointPrefab, transform.position, Quaternion.identity);
    }

    private Vector3 getSpawnPosition()
    {
        Vector3 spawnDirection;
        if(m_wayPointClone.transform.position != transform.position)
        {
            spawnDirection = (m_wayPointClone.transform.position - transform.position).normalized;
        }
        else
        {
            spawnDirection = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)).normalized;
        }

        Vector3 spawnPosition = transform.position;
        int distance = 1;
        while(m_selectionComponent.contains(spawnPosition))
        {
            spawnPosition += spawnDirection * distance;
            ++distance;
        }

        spawnPosition += spawnDirection * m_spawnOffSet; 

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

    public Unit spawnUnit(eUnitType unitType)
    {
        Unit newUnit = null;
        Vector3 spawnPosition = getSpawnPosition();
        
        if (!Map.Instance.isPositionOccupied(spawnPosition))
        {
            if(unitType == eUnitType.Harvester)
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
            Assert.IsNotNull(newUnit);

            if (m_wayPointClone.transform.position != transform.position)
            {
                Assert.IsTrue(m_wayPointClone.activeSelf);
                UnitStateHandler stateHandlerComponent = newUnit.GetComponent<UnitStateHandler>();
                Assert.IsNotNull(stateHandlerComponent);

                stateHandlerComponent.switchToState(eUnitState.MovingToNewPosition, Utilities.INVALID_ID, m_wayPointClone.transform.position);
            }

            return newUnit;
        }
        else
        {
            return null;
        }
    }
}