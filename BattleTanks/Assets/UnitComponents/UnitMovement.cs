using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float m_maxVelocity = 1.0f;
    [SerializeField]
    private float m_maxAcceleration = 5.0f;
    [SerializeField]
    private Vector3 m_velocity;
    [SerializeField]
    private float m_mass;

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
        if (m_positionToMoveTo.Count != 0)
        {
            Vector3 newHeading = Vector3.MoveTowards(transform.position, 
                new Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y), 
                m_maxAcceleration * Time.deltaTime);

            Vector3 acceleration = (newHeading - transform.position).normalized * m_maxVelocity - m_velocity;

            if (acceleration.magnitude > m_maxAcceleration)
                acceleration = acceleration.normalized * m_maxAcceleration;
        }

        //Bumping goes here

        //Apply acceleration to velocity and velocity to position

        if (m_positionToMoveTo.Count != 0)
        {
            if (reachedDestination())
            {
                m_positionToMoveTo.Dequeue();
            }
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