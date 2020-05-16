﻿using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    private float m_maxVelocity = 10.0f;
    [SerializeField]
    private UnityEngine.Vector3 m_velocity;
    [SerializeField]
    private float m_mass = 1.0f;
    [SerializeField]
    private bool m_bumping = true;
    [SerializeField]
    private float m_bumpRange = 0.7f;
    [SerializeField]
    private float m_dragStrength = 4.0f;
    //Just for viewing purposes
    public Vector2Int m_nextPos = new Vector2Int();
    public int m_count = 0;

    private Queue<Vector2Int> m_positionToMoveTo = new Queue<Vector2Int>();
    private Vector2Int m_finalDestination = new Vector2Int();
    private Unit m_unit = null;
    private float m_timeOfLastPath;
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
        m_count = m_positionToMoveTo.Count;
        if (m_positionToMoveTo.Count != 0)
            m_nextPos = m_positionToMoveTo.Peek();
        else
            m_nextPos = new Vector2Int(-1, -1);

        UnityEngine.Vector3 currentPosition = transform.position;
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.z));
        UnityEngine.Vector3 oldVelocity = m_velocity;

        UnityEngine.Vector3 finalMovement = UnityEngine.Vector3.zero;
        UnityEngine.Vector3 potentialFinalMovement = UnityEngine.Vector3.zero;
        UnityEngine.Vector3 pathingMovement = UnityEngine.Vector3.zero;
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
                UnityEngine.Vector3 newHeading = UnityEngine.Vector3.MoveTowards(currentPosition,
                    new UnityEngine.Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y),
                    m_maxVelocity);

                pathingMovement = (newHeading - currentPosition).normalized;
            }
        }
        else
        {
            //m_velocity -= m_velocity.normalized * Mathf.Max(0.0f, (m_velocity.magnitude / (m_dragStrength * 10.0f)) - 1.0f) * Time.deltaTime;
            m_velocity = UnityEngine.Vector3.zero;
        }

        UnityEngine.Vector3 potentialPosition = currentPosition + pathingMovement * m_maxVelocity * Time.deltaTime;

        if (m_bumping)
        {
            //Scenery bumping
            UnityEngine.Vector3 bumpingResult = UnityEngine.Vector3.zero;
            UnityEngine.Vector3 potentialBumpingResult = UnityEngine.Vector3.zero;
            //Find objects that are too close
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    if (!Map.Instance.isInBounds(roundedPosition.x + x, roundedPosition.y + y))
                    {
                        UnityEngine.Vector3 obstPosition = new UnityEngine.Vector3(roundedPosition.x + x, 1.0f, roundedPosition.y + y);

                        UnityEngine.Vector3 diff = currentPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            bumpingResult += diff.normalized / (diff.sqrMagnitude + 0.01f);

                        diff = potentialPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            potentialBumpingResult += diff.normalized / (diff.sqrMagnitude + 0.01f);
                    }
                    else if (Map.Instance.getPoint(roundedPosition.x + x, roundedPosition.y + y).scenery)
                    {
                        UnityEngine.Vector3 obstPosition = new UnityEngine.Vector3(roundedPosition.x + x, 1.0f, roundedPosition.y + y);

                        UnityEngine.Vector3 diff = currentPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            bumpingResult += diff.normalized * m_bumpRange / (diff.sqrMagnitude + 0.01f);

                        diff = potentialPosition - obstPosition;
                        if (diff.sqrMagnitude < m_bumpRange * m_bumpRange)
                            potentialBumpingResult += diff.normalized * m_bumpRange / (diff.sqrMagnitude + 0.01f);
                    }
                }
            }

            //Unit bumping
            UnityEngine.Vector3 unitResult = UnityEngine.Vector3.zero;
            UnityEngine.Vector3 potentialUnitResult = UnityEngine.Vector3.zero;
            List<int> closeUnits = new List<int>();
            GameManager.Instance.getFactionUnitsInRange(ref closeUnits, currentPosition, m_bumpRange, (int)m_unit.getFactionName());
            foreach (int id in closeUnits)
            {
                UnityEngine.Vector3 diff = currentPosition - GameManager.Instance.getUnit(id).getPosition();
                unitResult += diff.normalized * m_bumpRange / ((diff.sqrMagnitude + 0.01f) * m_mass);

                diff = potentialPosition - GameManager.Instance.getUnit(id).getPosition();
                potentialUnitResult += diff.normalized * m_bumpRange / ((diff.sqrMagnitude + 0.01f) * m_mass);
            }

            //Debug.Log("bumping then pathingMovement");
            //Debug.Log(bumpingResult);
            //Debug.Log(pathingMovement);

            //If the unit is bumping into other units and no walls are nearby, let it hit nearby waypoints without needing to get as close
            if (unitResult != UnityEngine.Vector3.zero && bumpingResult == UnityEngine.Vector3.zero &&
                (new UnityEngine.Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y) - transform.position).sqrMagnitude < 2.0f)
                m_positionToMoveTo.Dequeue();

            //Accumulate velocity change
            accumulate(ref bumpingResult, unitResult);
            accumulate(ref potentialBumpingResult, potentialUnitResult);

            if (bumpingResult != UnityEngine.Vector3.zero || potentialBumpingResult != UnityEngine.Vector3.zero)
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

        //Apply pathingMovement to velocity and velocity to position
        m_velocity = 0.5f * (finalMovement + potentialFinalMovement) * m_maxVelocity;
        m_velocity.y = 0.0f;
        
        transform.position += m_velocity * Time.deltaTime;
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
        m_timeOfLastPath = Time.time;
        m_positionToMoveTo.Clear();
        if(Map.Instance.isInBounds(position))
        {
            Vector2Int start = Utilities.convertToGridPosition(transform.position);
            Vector2Int end = Utilities.convertToGridPosition(position);

            m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), dangerAvoid, usageAvoid);
            if (m_positionToMoveTo.Count != 0)
                m_finalDestination = end;
        }
    }

    public void moveTo(Vector2Int position)
    {
        m_timeOfLastPath = Time.time;
        m_positionToMoveTo.Clear();
        if (Map.Instance.isInBounds(position))
        {
            Vector2Int start = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
            Vector2Int end = position;

            m_positionToMoveTo = Pathfinder.Instance.findPath(start, end, (int)m_unit.getFactionName(), dangerAvoid, usageAvoid);
            if (m_positionToMoveTo.Count != 0)
                m_finalDestination = end;
        }
    }

    public bool reachedDestination()
    {
        return m_positionToMoveTo.Count == 0;
    }

    public bool reachedWaypoint()
    {
        if (m_positionToMoveTo.Count == 0)
            return true;
        return (new UnityEngine.Vector3(m_positionToMoveTo.Peek().x, 1.0f, m_positionToMoveTo.Peek().y) - transform.position).sqrMagnitude < 0.2f;
    }

    public void stop()
    {
        m_positionToMoveTo = new Queue<Vector2Int>();
    }
}