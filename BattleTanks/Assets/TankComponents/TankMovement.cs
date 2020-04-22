using UnityEngine;
using UnityEngine.Assertions;

public class TankMovement : MonoBehaviour
{
    [SerializeField]
    private float m_movementSpeed = 0.0f;
    private Vector3 m_positionToMoveTo;
    private Tank m_tank = null;
    private bool m_startingPositionSet = false;

    private void Awake()
    {
        m_tank = GetComponent<Tank>();
        Assert.IsNotNull(m_tank);

        m_positionToMoveTo = transform.position;
    }

    private void Update()
    {
        if(!m_startingPositionSet)
        {
            Map.Instance.setStartingPosition(transform.position, m_tank.m_factionName, m_tank.m_ID);
            m_startingPositionSet = true;
        }

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
                Map.Instance.updatePositionOnMap(newPosition, transform.position, m_tank.m_factionName, m_tank.m_ID);
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