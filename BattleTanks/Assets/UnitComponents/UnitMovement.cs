using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float m_movementSpeed = 0.0f;
    [SerializeField]
    Vector2Int TEMPnextPos = new Vector2Int();
    private Queue<Vector2Int> m_positionToMoveTo = new Queue<Vector2Int>();
    private Unit m_unit = null;
    private bool m_startingPositionSet = false;

    public float dangerAvoid = 0.5f;
    public float usageAvoid = 1.0f;

    private void Awake()
    {
        m_unit = GetComponent<Unit>();
        Assert.IsNotNull(m_unit);
    }

    private void Update()
    {
        //if(!m_startingPositionSet)
        //{
        //    Map.Instance.setStartingPosition(transform.position, m_unit.getFactionName(), m_unit.getID());
        //    m_startingPositionSet = true;
        //}
        if (m_positionToMoveTo.Count != 0)
            TEMPnextPos = m_positionToMoveTo.Peek();
        //Assert.IsTrue(Map.Instance.isPositionOccupied(transform.position, m_unit.getID()));
        if (m_positionToMoveTo.Count != 0)
        {
            Vector3 newPosition = Vector3.MoveTowards(
                transform.position, new Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y), m_movementSpeed * Time.deltaTime);
            transform.position = newPosition;

            if (reachedDestination())
            {
                m_positionToMoveTo.Dequeue();
            }

            ////Movement on current grid cell
            //if (Map.Instance.isPositionOnOccupiedCell(newPosition, transform.position))
            //{   
            //    transform.position = newPosition;
            //}   
            ////Moving to new grid cell
            //else if (!Map.Instance.isPositionOccupied(newPosition))
            //{ 
            //    Map.Instance.updatePositionOnMap(newPosition, transform.position, m_unit.getFactionName(), m_unit.getID());
            //    transform.position = newPosition;
            //}
            //else
            //{
            //    //Grid cell to move to is occupied
            //    if (!Map.Instance.isPositionOnOccupiedCell(newPosition, transform.position) &&
            //        Map.Instance.isPositionOccupied(newPosition))
            //    {
            //        stop();
            //    }
            //}
        }
    } 

    public void moveTo(Vector3 position)
    {
        if(Map.Instance.isInBounds(position))
        {
            Vector2Int start = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
            Vector2Int end = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), dangerAvoid, usageAvoid);
        }
    }

    public bool reachedDestination()
    {
        if (m_positionToMoveTo.Count == 0)
            return true;
        return (new Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y) - transform.position).magnitude < 0.2f;
    }

    public void stop()
    {
        m_positionToMoveTo = new Queue<Vector2Int>();
    }
}