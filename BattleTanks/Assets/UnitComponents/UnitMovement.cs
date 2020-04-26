using UnityEngine;
using UnityEngine.Assertions;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float m_movementSpeed = 0.0f;
    private Vector3 m_positionToMoveTo;
    private Unit m_unit = null;
    private bool m_startingPositionSet = false;

    private void Awake()
    {
        m_unit = GetComponent<Unit>();
        Assert.IsNotNull(m_unit);

        m_positionToMoveTo = transform.position;
    }

    private void Update()
    {
        if(!m_startingPositionSet)
        {
            Map.Instance.setStartingPosition(transform.position, m_unit.m_factionName, m_unit.getID());
            m_startingPositionSet = true;
        }

        Assert.IsTrue(Map.Instance.isPositionOccupied(transform.position, m_unit.getID()));
        if (transform.position != m_positionToMoveTo)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, m_positionToMoveTo, m_movementSpeed * Time.deltaTime);

            //Movement on current grid cell
            if (Map.Instance.isPositionOnOccupiedCell(newPosition, transform.position))
            {   
                transform.position = newPosition;
            }
            //Moving to new grid cell
            else if (!Map.Instance.isPositionOccupied(newPosition))
            { 
                Map.Instance.updatePositionOnMap(newPosition, transform.position, m_unit.m_factionName, m_unit.getID());
                transform.position = newPosition;
            }
            else
            {
                //Grid cell to move to is occupied
                if (!Map.Instance.isPositionOnOccupiedCell(newPosition, transform.position) &&
                    Map.Instance.isPositionOccupied(newPosition))
                {
                    stop();
                }
            }
        }
    } 

    public void moveTo(Vector3 position)
    {
        if(Map.Instance.isInBounds(position))
        {
            m_positionToMoveTo = position;
        }
    }

    public bool reachedDestination()
    {
        return m_positionToMoveTo == transform.position;
    }

    public void stop()
    {
        m_positionToMoveTo = transform.position;
    }
}