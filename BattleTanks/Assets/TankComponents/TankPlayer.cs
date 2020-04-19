using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TankPlayer : MonoBehaviour
{
    //AI Stuff
    [SerializeField]
    public eAIState m_currentState;
    public eAIBehaviour m_behaviour;
    public int m_targetID = Utilities.INVALID_ID;

    private Tank m_tank = null;
    private TankMovement m_tankMovement = null;
    private TankShooting m_tankShooting = null;

    private void Start()
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
            case eAIState.SetDestinationToSafePosition:
                {
                    m_tankMovement.moveTo(PathFinding.Instance.getClosestSafePosition(8, m_tank));
                    m_currentState = eAIState.MovingToNewPosition;
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

    private void SendMessageToCommander(Vector2Int positionOnGrid, eAIUniMessageType messageType)
    {
        GraphPoint pointOnEnemy = Map.Instance.getPointOnMap(positionOnGrid);
        MessageToAIController message = new MessageToAIController(pointOnEnemy.tankID, positionOnGrid, eAIUniMessageType.EnemySpottedAtPosition,
            m_tank.m_ID, m_tank.m_factionName);

        GameManager.Instance.sendAIControllerMessage(message);
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
                if (result.sqrMagnitude <= m_tank.m_visibilityDistance * m_tank.m_visibilityDistance &&
                    Map.Instance.getPointOnMap(y, x).tankID == m_targetID)
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
}
