﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eUnitState
{
    AwaitingDecision = 0,
    SetNewDestination,
    SetAttackDestination,
    ShootingAtEnemy,
    MovingToNewPosition,
    SetDestinationToSafePosition
}

public class UnitStateHandler : MonoBehaviour
{
    [SerializeField]
    protected eUnitState m_currentState = eUnitState.AwaitingDecision;
    [SerializeField]
    protected int m_targetID = Utilities.INVALID_ID;
    [SerializeField]
    private float m_timeBetweenIdleCheck = 0.0f;

    private Unit m_unit = null;
    protected UnitMovement m_tankMovement = null;
    private UnitAttack m_tankShooting = null;
    private bool m_attackMove = false;
    private float m_elaspedTime = 0.0f;

    protected virtual void Awake()
    {
        m_unit = GetComponent<Unit>();
        Assert.IsNotNull(m_unit);

        m_tankMovement = GetComponent<UnitMovement>();
        Assert.IsNotNull(m_tankMovement);

        m_tankShooting = GetComponent<UnitAttack>();
        Assert.IsNotNull(m_tankShooting);
    }

    protected virtual void Update()
    {
        switch (m_currentState)
        {
            case eUnitState.AwaitingDecision:
                {
                    m_elaspedTime += Time.deltaTime;
                    if(m_elaspedTime >= m_timeBetweenIdleCheck)
                    {
                        m_elaspedTime = 0.0f;

                        int targetID = Utilities.INVALID_ID;
                        Vector3 targetPosition;
                        bool enemySpotted = getClosestVisibleTarget(out targetID, out targetPosition);
                        if (m_unit.getUnitType() == eUnitType.Attacker && enemySpotted)
                        {
                            m_targetID = targetID;
                            m_currentState = eUnitState.MovingToNewPosition;
                            m_tankMovement.moveTo(targetPosition);
                        }
                        else if(m_unit.getUnitType() == eUnitType.Harvester && enemySpotted)
                        {
                            Debug.Log("Set position to safe area");
                            m_currentState = eUnitState.SetDestinationToSafePosition;
                        }
                    }
                }
                break;
            case eUnitState.MovingToNewPosition:
                {
                    Vector3 enemyPosition = new Vector3();
                    if(m_attackMove)
                    {
                        int targetID = Utilities.INVALID_ID;
                        Vector3 targetPosition;

                        if (getClosestVisibleTarget(out targetID, out targetPosition))
                        {
                            m_attackMove = false;
                            m_targetID = targetID;
                            m_currentState = eUnitState.MovingToNewPosition;
                            m_tankMovement.moveTo(targetPosition);
                        }
                    }
                    else
                    {
                        if (m_targetID != Utilities.INVALID_ID && isTargetInVisibleSight(out enemyPosition))
                        {
                            m_tankMovement.moveTo(enemyPosition);

                            if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                            {
                                m_tankMovement.stop();
                                m_currentState = eUnitState.ShootingAtEnemy;
                            }
                        }
                        else if (m_targetID != Utilities.INVALID_ID && !isTargetInVisibleSight(out enemyPosition))
                        {
                            m_tankMovement.stop();
                            m_targetID = Utilities.INVALID_ID;
                        }
                        else
                        {
                            if (m_tankMovement.reachedDestination())
                            {
                                m_currentState = eUnitState.AwaitingDecision;
                            }
                        }
                    }
                }
                break;
            case eUnitState.ShootingAtEnemy:
                {
                    Vector3 enemyPosition = new Vector3();
                    if (isTargetInVisibleSight(out enemyPosition))
                    {
                        if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                        {
                            m_tankMovement.stop();
                            m_tankShooting.attackPosition(enemyPosition);
                        }
                        else
                        {
                            m_tankMovement.moveTo(enemyPosition);
                        }
                    }
                    else
                    {
                        m_tankMovement.stop();
                        m_targetID = Utilities.INVALID_ID;
                        m_currentState = eUnitState.AwaitingDecision;
                    }
                }
                break;
            case eUnitState.SetDestinationToSafePosition:
                {

                }
                break;
        }
    }

    public void switchToState(eUnitState state, int targetID, Vector3 position)
    {
        switch (state)
        {
            case eUnitState.ShootingAtEnemy:
            case eUnitState.MovingToNewPosition:
                {
                    m_targetID = targetID;
                    m_currentState = state;
                    m_tankMovement.moveTo(position);
                }
                break;
            case eUnitState.SetNewDestination:
                {
                    m_targetID = targetID;
                    m_currentState = eUnitState.MovingToNewPosition;
                    m_tankMovement.moveTo(position);
                }
                break;
            case eUnitState.SetAttackDestination:
                {
                    m_targetID = targetID;
                    m_currentState = eUnitState.MovingToNewPosition;
                    m_tankMovement.moveTo(position);
                    m_attackMove = true;
                }
                break;
        }
    }

    private bool isTargetInVisibleSight(out Vector3 enemyPosition)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        iRectangle searchableRect = new iRectangle(positionOnGrid, m_unit.getVisibilityDistance());

        for (int y = searchableRect.m_bottom; y <= searchableRect.m_top; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                Vector2Int result = positionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if (pointOnMap == null)
                {
                    continue;
                }

                if (pointOnMap.unitID == m_targetID &&
                    result.sqrMagnitude <= m_unit.getVisibilityDistance() * m_unit.getVisibilityDistance())
                {
                    Unit unit = GameManager.Instance.getUnit(m_targetID);
                    Assert.IsNotNull(unit);

                    enemyPosition = new Vector3(unit.transform.position.x, 0, unit.transform.position.z);
                    return true;
                }
            }
        }

        enemyPosition = new Vector3();
        return false;
    }

    private bool getClosestVisibleTarget(out int enemyID, out Vector3 enemyPosition)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        iRectangle searchableRect = new iRectangle(positionOnGrid, m_unit.getVisibilityDistance());
        int closestTargetID = Utilities.INVALID_ID;
        float distance = float.MaxValue;

        for (int y = searchableRect.m_bottom; y <= searchableRect.m_top; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                Vector2Int result = positionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if (pointOnMap == null)
                {
                    continue;
                }

                if (result.sqrMagnitude <= m_unit.getVisibilityDistance() * m_unit.getVisibilityDistance() &&
                    pointOnMap.isOccupiedByEnemy(m_unit.m_factionName))
                {
                    float d = (positionOnGrid - new Vector2Int(x, y)).magnitude;
                    if (d < distance)
                    {
                        closestTargetID = pointOnMap.unitID;
                        distance = d;
                    }
                }
            }
        }

        if (closestTargetID != Utilities.INVALID_ID)
        {
            Unit enemy = GameManager.Instance.getUnit(closestTargetID);
            Assert.IsNotNull(enemy);

            enemyID = enemy.getID();
            enemyPosition = enemy.transform.position;
            return true;
        }
        else
        {
            enemyID = Utilities.INVALID_ID;
            enemyPosition = Utilities.INVALID_POSITION;
            return false;
        }
    }
}