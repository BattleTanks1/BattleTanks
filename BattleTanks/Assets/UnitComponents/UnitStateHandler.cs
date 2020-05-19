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
    AttackMovingToNewPosition,
    SetDestinationToSafePosition,
    InUseBySecondaryState
}

public class UnitStateHandler : MonoBehaviour
{
    [SerializeField]
    protected eUnitState m_currentState = eUnitState.AwaitingDecision;
    [SerializeField]
    protected int m_targetID = Utilities.INVALID_ID;

    private Unit m_unit = null;
    protected UnitMovement m_tankMovement = null;
    private UnitAttack m_tankShooting = null;

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
                    int targetID = Utilities.INVALID_ID;
                    Vector3 targetPosition;
                    if (getClosestVisibleTarget(out targetID, out targetPosition))
                    {
                        switch (m_unit.getUnitType())
                        {
                            case eUnitType.Attacker:
                                switchToState(eUnitState.SetDestination, targetID, targetPosition);
                                break;
                            case eUnitType.Harvester:
                                switchToState(eUnitState.SetDestinationToSafePosition);
                                break;
                            default:
                                Assert.IsTrue(false);
                                break;
                        }
                    }
                }
                break;
            case eUnitState.MovingToNewPosition:
                {
                    if (m_tankMovement.reachedDestination())
                    {
                        Debug.Log("Reached Destination");
                    }

                    Vector3 enemyPosition = new Vector3();
                    if (m_targetID != Utilities.INVALID_ID && isTargetInVisibleSight(out enemyPosition))
                    {
                        if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                        {
                            switchToState(eUnitState.AttackingEnemy, m_targetID);
                        }
                        else
                        {
                            m_tankMovement.moveTo(enemyPosition);
                        }
                    }
                    else if (m_targetID != Utilities.INVALID_ID)
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
                break;
            case eUnitState.AttackMovingToNewPosition:
                {
                    int targetID = Utilities.INVALID_ID;
                    Vector3 targetPosition;
                    if (getClosestVisibleTarget(out targetID, out targetPosition))
                    {
                        switchToState(eUnitState.SetDestination, targetID, targetPosition);
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
                }
                break;
            case eUnitState.SetAttackDestination:
                {
                    m_currentState = eUnitState.AttackMovingToNewPosition;
                    m_tankMovement.moveTo(position);
                }
                break;
            case eUnitState.SetDestinationToSafePosition:
                {
                    m_tankMovement.moveTo(Pathfinder.Instance.getSafePosition(transform.position, (int)m_unit.getFactionName(), m_unit.getScaredValue()));
                    m_currentState = eUnitState.MovingToNewPosition;
                }
                break;
        }
    }

    private bool isTargetInVisibleSight(out Vector3 enemyPosition)
    {
        Assert.IsTrue(m_targetID != Utilities.INVALID_ID);
        List<Unit> opposingFactionUnits = GameManager.Instance.getOpposingFactionUnits(m_unit.getFactionName());
        Assert.IsNotNull(opposingFactionUnits);

        foreach (Unit unit in opposingFactionUnits)
        {
            if(unit.getID() == m_targetID &&
                (unit.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(m_unit.getVisibilityDistance(), 2))
            {
                enemyPosition = unit.transform.position;
                return true;
            }
        }

        enemyPosition = Utilities.INVALID_POSITION;
        return false;
    }

    private bool getClosestVisibleTarget(out int enemyID, out Vector3 enemyPosition)
    {
        List<Unit> opposingFactionUnits = GameManager.Instance.getOpposingFactionUnits(m_unit.getFactionName());
        Assert.IsNotNull(opposingFactionUnits);
        Unit targetUnit = null;
        float closestUnitDistance = float.MaxValue;

        foreach (Unit unit in opposingFactionUnits)
        {
            float distanceToUnit = (unit.transform.position - transform.position).sqrMagnitude;
            if (distanceToUnit <= Mathf.Pow(m_unit.getVisibilityDistance(), 2) && distanceToUnit < closestUnitDistance)
            {
                targetUnit = unit;
                closestUnitDistance = distanceToUnit;
            }
        }

        if(targetUnit)
        {
            enemyID = targetUnit.getID();
            enemyPosition = targetUnit.transform.position;
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