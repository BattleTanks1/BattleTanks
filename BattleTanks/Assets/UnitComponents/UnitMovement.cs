using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float m_maxVelocity = 10.0f;
    public Vector3 m_velocity { get; private set; }
    [SerializeField]
    private float m_mass = 1.0f;
    [SerializeField]
    private bool m_influence = true;
    [SerializeField]
    private bool m_bumping = true;
    [SerializeField]
    private float m_bumpRange = 0.7f;

    private Queue<Vector2Int> m_positionToMoveTo = new Queue<Vector2Int>();
    private Vector2Int m_finalDestination = new Vector2Int();
    private Unit m_unit = null;
    private float m_timeOfLastPath = 0.0f;
    private bool m_startingPositionSet = false;

    private UnitStateHandler m_unitState;

    private void Awake()
    {
        m_unit = GetComponent<Unit>();
        Assert.IsNotNull(m_unit);
        m_unitState = GetComponent<UnitStateHandler>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.z));
        Vector3 oldVelocity = m_velocity;

        Vector3 finalMovement = Vector3.zero;
        Vector3 potentialFinalMovement = Vector3.zero;
        Vector3 pathingMovement = Vector3.zero;
        //Unit movement attempt
        //Note this is only actually added to the next velocity last but the value is used by subsequent parts to estimate next location
        if (m_positionToMoveTo.Count != 0)
        {
            //m_velocity -= m_velocity.normalized * Mathf.Max(0.0f, (m_velocity.magnitude / m_dragStrength) - 0.01f) * Time.deltaTime;
            if (reachedWaypoint())
            {
                m_positionToMoveTo.Dequeue();
            }
            else
            {
                Vector3 newHeading = Vector3.MoveTowards(currentPosition,
                    new Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y),
                    m_maxVelocity);

                pathingMovement = (newHeading - currentPosition).normalized;
            }
        }
        else
        {
            m_velocity = Vector3.zero;
        }

        Vector3 potentialPosition = currentPosition + pathingMovement * m_maxVelocity * Time.deltaTime;

        if (m_bumping)
        {
            //Scenery bumping
            Vector3 bumpingResult = Vector3.zero;
            Vector3 potentialBumpingResult = Vector3.zero;
            bool wallsNearby = false;
            //Find objects that are too close
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    if (!Map.Instance.isInBounds(roundedPosition.x + x, roundedPosition.y + y))
                    {
                        wallsNearby = true;
                        Vector3 obstPosition = new Vector3(roundedPosition.x + x, 1.0f, roundedPosition.y + y);

                        Vector3 diff = currentPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            bumpingResult += diff.normalized / (diff.sqrMagnitude + 0.01f);

                        diff = potentialPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            potentialBumpingResult += diff.normalized / (diff.sqrMagnitude + 0.01f);
                    }
                    else if (Map.Instance.getPoint(roundedPosition.x + x, roundedPosition.y + y).scenery)
                    {
                        wallsNearby = true;
                        Vector3 obstPosition = new Vector3(roundedPosition.x + x, 1.0f, roundedPosition.y + y);

                        Vector3 diff = currentPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            bumpingResult += diff.normalized * m_bumpRange / (diff.sqrMagnitude + 0.01f);

                        diff = potentialPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            potentialBumpingResult += diff.normalized * m_bumpRange / (diff.sqrMagnitude + 0.01f);
                    }
                }
            }

            //Unit bumping
            Vector3 unitResult = Vector3.zero;
            Vector3 potentialUnitResult = Vector3.zero;
            List<int> closeUnits = new List<int>();
            GameManager.Instance.getFactionUnitsInRange(ref closeUnits, currentPosition, m_bumpRange, (int)m_unit.getFactionName());
            foreach (int id in closeUnits)
            {
                Vector3 diff = currentPosition - GameManager.Instance.getUnit(id).getPosition();
                unitResult += diff.normalized * m_bumpRange / ((diff.sqrMagnitude + 0.01f) * m_mass);

                diff = potentialPosition - GameManager.Instance.getUnit(id).getPosition();
                potentialUnitResult += diff.normalized * m_bumpRange / ((diff.sqrMagnitude + 0.01f) * m_mass);
            }

            //Debug.Log("bumping then pathingMovement");
            //Debug.Log(bumpingResult);
            //Debug.Log(pathingMovement);

            //If the unit is bumping into other units and no walls are nearby, let it hit nearby waypoints without needing to get as close
            if (unitResult != Vector3.zero && wallsNearby && m_positionToMoveTo.Count != 0 &&
                (new Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y) - transform.position).sqrMagnitude < 1.0f)
                m_positionToMoveTo.Dequeue();

            //Accumulate velocity change
            accumulate(ref bumpingResult, unitResult);
            accumulate(ref potentialBumpingResult, potentialUnitResult);

            //If the unit is regularly bumping into things then recalculate the path if enough time has passed
            if (bumpingResult != Vector3.zero || potentialBumpingResult != Vector3.zero)
            {
                if (Time.time - m_timeOfLastPath > 1.0f && m_positionToMoveTo.Count != 0)
                {
                    moveTo(m_finalDestination);
                }
            }

            accumulate(ref bumpingResult, pathingMovement);
            accumulate(ref potentialBumpingResult, pathingMovement);
            finalMovement = bumpingResult;
            potentialFinalMovement = potentialBumpingResult;
        }
        else
        {
            finalMovement = pathingMovement;
            potentialFinalMovement = pathingMovement;
        }

        //Apply pathingMovement to velocity
        m_velocity = 0.5f * (finalMovement + potentialFinalMovement) * m_maxVelocity;
    }

    //Adds a vector to another, capping the length of the second such that the result's length is 1 or less
    private void accumulate(ref Vector3 acc, Vector3 add)
    {
        if (acc.magnitude == 1.0f)
            return;

        float dot = Vector3.Dot(acc, add);
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

    private bool reachedWaypoint()
    {
        if (m_positionToMoveTo.Count == 0)
            return true;
        return (new Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y) - transform.position).sqrMagnitude < 0.1f;
    }

    public void moveTo(Vector3 position)
    {
        //Catch for numerous calls for the same location
        if (m_finalDestination == Utilities.convertToGridPosition(position) && 
            Time.time - m_timeOfLastPath > 0.5f)
            return;

        m_timeOfLastPath = Time.time;
        m_positionToMoveTo.Clear();
        if(Map.Instance.isInBounds(position))
        {
            Vector2Int start = Utilities.convertToGridPosition(transform.position);
            Vector2Int end = Utilities.convertToGridPosition(position);

            if (m_influence)
                m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), m_unit.m_dangerAvoid, m_unit.m_usageAvoid);
            else
                m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), 0.0f, 0.0f);

            if (m_positionToMoveTo.Count != 0)
                m_finalDestination = end;
        }
    }

    public void moveTo(Vector2Int position)
    {
        //Catch for numerous calls for the same location
        if (m_finalDestination == position && Time.time - m_timeOfLastPath > 0.5f)
            return;

        m_timeOfLastPath = Time.time;
        m_positionToMoveTo.Clear();

        if (Map.Instance.isInBounds(position))
        {
            Vector2Int start = Utilities.convertToGridPosition(transform.position);
            Vector2Int end = position;

            if (m_influence)
                m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), m_unit.m_dangerAvoid, m_unit.m_usageAvoid);
            else
                m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), 0.0f, 0.0f);

            if (m_positionToMoveTo.Count != 0)
                m_finalDestination = end;
        }
    }

    public void moveStraightTowards(Vector3 position)
    {
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        m_positionToMoveTo.Clear();
        m_positionToMoveTo.Enqueue(roundedPosition);
        m_finalDestination = roundedPosition;
    }

    public bool reachedDestination()
    {
        return m_positionToMoveTo.Count == 0;
    }

    public void stop()
    {
        m_positionToMoveTo = new Queue<Vector2Int>();
    }
}