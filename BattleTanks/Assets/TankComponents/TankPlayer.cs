using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TankPlayer : MonoBehaviour
{
    [SerializeField]
    private eAIState m_currentState;
    [SerializeField]
    private int m_targetID = Utilities.INVALID_ID;

    private Tank m_tank = null;
    private TankMovement m_tankMovement = null;
    private TankShooting m_tankShooting = null;


    private void Awake()
    {
        m_tank = GetComponent<Tank>();
        Assert.IsNotNull(m_tank);

        m_tankMovement = GetComponent<TankMovement>();
        Assert.IsNotNull(m_tankMovement);

        m_tankShooting = GetComponent<TankShooting>();
        Assert.IsNotNull(m_tankShooting);
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case eAIState.AwaitingDecision:
                {
                    int targetID = Utilities.INVALID_ID;
                    Vector3 targetPosition;
                    if(isEnemyInVisibleSight(out targetID, out targetPosition))
                    {
                        Assert.IsTrue(targetID != Utilities.INVALID_ID);
                        Assert.IsTrue(targetPosition != Utilities.INVALID_POSITION);

                        m_targetID = targetID;
                        m_currentState = eAIState.MovingToNewPosition;
                        m_tankMovement.moveTo(targetPosition);
                    }
                }
                break;
            case eAIState.MovingToNewPosition:
                {
                    Vector3 enemyPosition = new Vector3();
                    if (m_targetID != Utilities.INVALID_ID && isTargetInVisibleSight(out enemyPosition))
                    {
                        m_tankMovement.moveTo(enemyPosition);

                        if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                        {
                            m_tankMovement.stop();
                            m_currentState = eAIState.ShootingAtEnemy;
                        }
                    }
                    else
                    {
                        if (m_tankMovement.reachedDestination())
                        {
                            m_currentState = eAIState.AwaitingDecision;
                        }
                    }
                }
                break;
            case eAIState.ShootingAtEnemy:
                {
                    Vector3 enemyPosition = new Vector3();
                    if (isTargetInVisibleSight(out enemyPosition))
                    {
                        if (m_tankShooting.isTargetInAttackRange(enemyPosition))
                        {
                            m_tankMovement.stop();
                            m_tankShooting.FireAtPosition(enemyPosition);
                        }
                        else
                        {
                            m_tankMovement.moveTo(enemyPosition);
                        }
                    }
                    else
                    {
                        m_targetID = Utilities.INVALID_ID;
                        m_currentState = eAIState.AwaitingDecision;
                    }
                }
                break;
        }
    }

    public void receiveMessage(MessageToAIUnit message)
    {
        switch (message.m_messageType)
        {
            case eAIState.ShootingAtEnemy:
                {
                    m_targetID = message.m_targetID;
                    m_currentState = message.m_messageType;
                    m_tankMovement.moveTo(Utilities.convertToWorldPosition(message.m_lastTargetPosition));
                }
                break;
            case eAIState.MovingToNewPosition:
                {
                    m_targetID = message.m_targetID;
                    m_currentState = eAIState.MovingToNewPosition;
                    m_tankMovement.moveTo(Utilities.convertToWorldPosition(message.m_lastTargetPosition));
                }
                break;
        }
    }

    private bool isTargetInVisibleSight(out Vector3 enemyPosition)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        iRectangle searchableRect = new iRectangle(positionOnGrid, m_tank.m_visibilityDistance);

        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                Vector2Int result = positionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if(pointOnMap == null)
                {
                    continue;
                }

                if (pointOnMap.tankID == m_targetID &&
                    result.sqrMagnitude <= m_tank.m_visibilityDistance * m_tank.m_visibilityDistance)
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

    private bool isEnemyInVisibleSight(out int enemyID, out Vector3 enemyPosition)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        iRectangle searchableRect = new iRectangle(positionOnGrid, m_tank.m_visibilityDistance);

        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                Vector2Int result = positionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if(pointOnMap == null)
                {
                    continue;
                }

                if (result.sqrMagnitude <= m_tank.m_visibilityDistance * m_tank.m_visibilityDistance &&
                    pointOnMap.tankID != Utilities.INVALID_ID && 
                    pointOnMap.tankFactionName != m_tank.m_factionName)
                {
                    Tank enemy = GameManager.Instance.getTank(pointOnMap.tankID);
                    Assert.IsNotNull(enemy);
                    if(!enemy)
                    {
                        continue;
                    }

                    enemyID = enemy.m_ID;
                    enemyPosition = enemy.transform.position;
                    return true;
                }
            }
        }

        enemyID = Utilities.INVALID_ID;
        enemyPosition = Utilities.INVALID_POSITION;
        return false;
    }
}
