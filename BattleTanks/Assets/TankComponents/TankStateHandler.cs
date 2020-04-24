using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eTankState
{
    AwaitingDecision = 0,
    SetNewDestination,
    SetAttackDestination,
    ShootingAtEnemy,
    MovingToNewPosition,
    SetDestinationToSafePosition
}

public class TankStateHandler : MonoBehaviour
{
    [SerializeField]
    private eTankState m_currentState = eTankState.AwaitingDecision;
    [SerializeField]
    private int m_targetID = Utilities.INVALID_ID;

    private Unit m_unit = null;
    private UnitMovement m_tankMovement = null;
    private UnitAttack m_tankShooting = null;
    private bool m_attackMove = false;

    private void Awake()
    {
        m_unit = GetComponent<Unit>();
        Assert.IsNotNull(m_unit);

        m_tankMovement = GetComponent<UnitMovement>();
        Assert.IsNotNull(m_tankMovement);

        m_tankShooting = GetComponent<UnitAttack>();
        Assert.IsNotNull(m_tankShooting);
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case eTankState.AwaitingDecision:
                {
                    int targetID = Utilities.INVALID_ID;
                    Vector3 targetPosition;

                    if (getClosestVisibleTarget(out targetID, out targetPosition))
                    {
                        m_targetID = targetID;
                        m_currentState = eTankState.MovingToNewPosition;
                        m_tankMovement.moveTo(targetPosition);
                    }
                }
                break;
            case eTankState.MovingToNewPosition:
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
                            m_currentState = eTankState.MovingToNewPosition;
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
                                m_currentState = eTankState.ShootingAtEnemy;
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
                                m_currentState = eTankState.AwaitingDecision;
                            }
                        }
                    }
                }
                break;
            case eTankState.ShootingAtEnemy:
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
                        m_currentState = eTankState.AwaitingDecision;
                    }
                }
                break;
        }
    }

    public void switchToState(eTankState state, int targetID, Vector3 position)
    {
        switch (state)
        {
            case eTankState.ShootingAtEnemy:
            case eTankState.MovingToNewPosition:
                {
                    m_targetID = targetID;
                    m_currentState = state;
                    m_tankMovement.moveTo(position);
                }
                break;
            case eTankState.SetNewDestination:
                {
                    m_targetID = targetID;
                    m_currentState = eTankState.MovingToNewPosition;
                    m_tankMovement.moveTo(position);
                }
                break;
            case eTankState.SetAttackDestination:
                {
                    m_targetID = targetID;
                    m_currentState = eTankState.MovingToNewPosition;
                    m_tankMovement.moveTo(position);
                    m_attackMove = true;
                }
                break;
        }
    }

    private bool isTargetInVisibleSight(out Vector3 enemyPosition)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        iRectangle searchableRect = new iRectangle(positionOnGrid, m_unit.m_visibilityDistance);

        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
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
                    result.sqrMagnitude <= m_unit.m_visibilityDistance * m_unit.m_visibilityDistance)
                {
                    Vector3 position = GameManager.Instance.getTankPosition(m_targetID);
                    Assert.IsTrue(position != Utilities.INVALID_POSITION);

                    enemyPosition = new Vector3(position.x, 0, position.z);
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
        iRectangle searchableRect = new iRectangle(positionOnGrid, m_unit.m_visibilityDistance);
        int closestTargetID = Utilities.INVALID_ID;
        float distance = float.MaxValue;

        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                Vector2Int result = positionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if (pointOnMap == null)
                {
                    continue;
                }

                if (result.sqrMagnitude <= m_unit.m_visibilityDistance * m_unit.m_visibilityDistance &&
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

            enemyID = enemy.m_ID;
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