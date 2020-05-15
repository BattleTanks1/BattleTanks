using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float m_maxVelocity = 10.0f;
    [SerializeField]
    private float m_maxAcceleration = 10.0f;
    [SerializeField]
    private UnityEngine.Vector3 m_velocity;
    [SerializeField]
    private float m_mass;
    [SerializeField]
    private float m_unitBumpRange = 2.0f;

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
        UnityEngine.Vector3 currentPosition = transform.position;
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.z));
        UnityEngine.Vector3 oldVelocity = m_velocity;

        //Scenery bumping
        UnityEngine.Vector3 bumpingResult = new UnityEngine.Vector3();
        //Find objects that are too close
        for (int x = -1; x < 2; ++x)
        {
            for (int y = -1; y < 2; ++y)
            {
                if (!Map.Instance.isInBounds(roundedPosition.x + x, roundedPosition.y + y) 
                    || Map.Instance.getPoint(roundedPosition.x + x, roundedPosition.y + y).scenery)
                {
                    UnityEngine.Vector3 obstPosition = new UnityEngine.Vector3(roundedPosition.x + x, 1.0f, roundedPosition.y + y);
                    UnityEngine.Vector3 diff = currentPosition - obstPosition;
                    bumpingResult += diff.normalized / (diff.sqrMagnitude + 0.01f);
                }
            }
        }

        //Unit bumping
        UnityEngine.Vector3 unitResult = new UnityEngine.Vector3();
        List<int> closeUnits = new List<int>();
        GameManager.Instance.getFactionUnitsInRange(ref closeUnits, currentPosition, m_unitBumpRange, (int)m_unit.getFactionName());
        foreach (int id in closeUnits)
        {
            UnityEngine.Vector3 diff = currentPosition - GameManager.Instance.getUnit(id).getPosition();
            unitResult += diff.normalized * m_unitBumpRange / ((diff.sqrMagnitude + 0.01f) * m_mass);
        }

        //Accumulate velocity change
        accumulate(ref bumpingResult, unitResult);

        UnityEngine.Vector3 acceleration = new UnityEngine.Vector3();
        //Unit movement attempt
        if (m_positionToMoveTo.Count != 0)
        {
            UnityEngine.Vector3 newHeading = UnityEngine.Vector3.MoveTowards(currentPosition,
                new UnityEngine.Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y),
                m_maxAcceleration * Time.deltaTime);

            acceleration = (newHeading - currentPosition).normalized * m_maxVelocity - m_velocity;

            if (acceleration.magnitude > m_maxAcceleration)
                acceleration = acceleration.normalized * m_maxAcceleration;
        }

        //Apply acceleration to velocity and velocity to position
        m_velocity += bumpingResult + acceleration * Time.deltaTime;
        if (m_velocity.sqrMagnitude > m_maxVelocity)
            m_velocity = m_velocity.normalized * m_maxVelocity;
        m_velocity.y = 0.0f;

        transform.position += 0.5f * (m_velocity + oldVelocity) * Time.deltaTime;

        //If destination reached 
        if (m_positionToMoveTo.Count != 0)
        {
            if (reachedDestination())
            {
                m_positionToMoveTo.Dequeue();
            }
        }
    }

    //Adds a vector to another, capping the length of the second such that the result's length is 1 or less
    private void accumulate(ref UnityEngine.Vector3 acc, UnityEngine.Vector3 add)
    {
        if (acc.magnitude == 1.0f)
            return;

        float dot = UnityEngine.Vector3.Dot(acc, add);
        float root = Mathf.Sqrt((dot * dot) - (add.sqrMagnitude * (acc.sqrMagnitude - 1)));
        float A = (-dot + root) / (acc.sqrMagnitude - 1);
        float B = (-dot - root) / (acc.sqrMagnitude - 1);

        if (A >= 0.0f)
        {
            A = Mathf.Min(1.0f, A);
            acc = acc + (add * A);
            acc = acc.normalized;
            return;
        }
        else if (B >= 0.0f)
        {
            B = Mathf.Min(1.0f, B);
            acc = acc + (add * B);
            acc = acc.normalized;
            return;
        }
    }

    public void moveTo(UnityEngine.Vector3 position)
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
        return (new UnityEngine.Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y) - transform.position).magnitude < 0.2f;
    }

    public void stop()
    {
        m_positionToMoveTo = new Queue<Vector2Int>();
    }
}