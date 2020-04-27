using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eUnitState
{
    AwaitingDecision = 0,
    SetDestination,
    SetAttackDestination,
    AttackingEnemy,
    MovingToNewPosition,
    SetDestinationToSafePosition,
    InUseBySecondaryState
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
                            switchToState(eUnitState.MovingToNewPosition, targetID, targetPosition);
                        }
                        else if(m_unit.getUnitType() == eUnitType.Harvester && enemySpotted)
                        {
                            switchToState(eUnitState.SetDestinationToSafePosition);
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
                            switchToState(eUnitState.SetDestination, targetID, targetPosition);
                        }
                    }
                    else
                    {
                        if (m_targetID != Utilities.INVALID_ID && isTargetInVisibleSight(out enemyPosition))
                        {
                            m_tankMovement.moveTo(enemyPosition);

                            if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                            {
                                switchToState(eUnitState.AttackingEnemy, m_targetID);
                            }
                        }
                        else if (m_targetID != Utilities.INVALID_ID && !isTargetInVisibleSight(out enemyPosition))
                        {
                            switchToState(eUnitState.AwaitingDecision);
                        }
                        else
                        {
                            if (m_tankMovement.reachedDestination())
                            {
                                switchToState(eUnitState.AwaitingDecision);
                            }
                        }
                    }
                }
                break;
            case eUnitState.AttackingEnemy:
                {
                    Vector3 enemyPosition = new Vector3();
                    if (isTargetInVisibleSight(out enemyPosition))
                    {
                        if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                        {
                            m_tankShooting.attackPosition(enemyPosition);
                        }
                        else
                        {
                            switchToState(eUnitState.SetAttackDestination, m_targetID, enemyPosition);
                        }
                    }
                    else
                    {
                        switchToState(eUnitState.AwaitingDecision);
                    }
                }
                break;
        }
    }

    public void switchToState(eUnitState newState, int targetID = Utilities.INVALID_ID, Vector3 position = new Vector3())
    {
        m_targetID = targetID;
        m_currentState = newState;

        switch (newState)
        {
            case eUnitState.AwaitingDecision:
            case eUnitState.AttackingEnemy:
                m_tankMovement.stop();
                break;
            case eUnitState.MovingToNewPosition:
                m_tankMovement.moveTo(position);
                break;
            case eUnitState.SetDestination:
                {
                    m_currentState = eUnitState.MovingToNewPosition;
                    m_tankMovement.moveTo(position);
                    m_attackMove = false;
                }
                break;
            case eUnitState.SetAttackDestination:
                {
                    m_currentState = eUnitState.MovingToNewPosition;
                    m_tankMovement.moveTo(position);
                    m_attackMove = true;
                }
                break;
            case eUnitState.SetDestinationToSafePosition:
                {
                    m_tankMovement.moveTo(PathFinding.Instance.getClosestSafePosition(8, m_unit));
                    m_currentState = eUnitState.MovingToNewPosition;
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
                    pointOnMap.isOccupiedByEnemy(m_unit.getFactionName()))
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