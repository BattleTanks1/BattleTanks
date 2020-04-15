using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//http://lecturer.ukdw.ac.id/~mahas/dossier/gameng_AIFG.pdf
//https://www.reddit.com/r/gamedev/comments/9onssu/where_can_i_learn_more_about_rts_ai/

//http://dl.booktolearn.com/ebooks2/computer/artificialintelligence/9781138483972_AI_for_Games_3rd_a694.pdf

public class Transition
{
    //When conditions are met - it is said to trigger
    //When transition goes to new state - it has fired
}

public enum eAIUniMessageType
{
    EnemySpottedAtPosition = 0,
    LostSightOfEnemy
}

public class MessageToAIController
{
    public MessageToAIController(int targetID, Vector2Int lastTargetPosition, eAIUniMessageType messageType, int senderID, eFactionName senderFaction)
    {
        m_targetID = targetID;
        m_lastTargetPosition = lastTargetPosition;
        m_messageType = messageType;
        m_senderID = senderID;
        m_senderFaction = senderFaction;
    }

    public MessageToAIController(int targetID, eAIUniMessageType messageType, eFactionName senderFaction)
    {
        m_targetID = targetID;
        m_messageType = messageType;
    }

    public Vector2Int m_lastTargetPosition { get; private set; }
    public int m_targetID { get; private set; }
    public eAIUniMessageType m_messageType { get; private set; }
    public int m_senderID { get; private set; }
    public eFactionName m_senderFaction { get; private set; }
}

public enum eAIState
{
    AwaitingDecision = 0,
    ShootingAtEnemy,
    MovingToNewPosition,
    SetDestinationToSafePosition,
}

public class AITank : MonoBehaviour
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
        //if (InfluenceMap.Instance.isPositionInThreat(m_tank))
        //{
        //    m_currentState = eAIState.SetDestinationToSafePosition;
        //}

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
                    if(m_targetID != Utilities.INVALID_ID && isTargetInVisibleSight(out enemyPosition))
                    {
                        m_tankMovement.moveTo(enemyPosition);

                        if(m_tankShooting.isTargetInAttackRange(enemyPosition))
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
                    if(isTargetInVisibleSight(out enemyPosition))
                    {
                        if(m_tankShooting.isTargetInAttackRange(enemyPosition))
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
        switch(message.m_messageType)
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
                    enemyPosition = new Vector3(x, 0, y);
                    return true;
                }
            }
        }

        enemyPosition = new Vector3();
        return false;
    }
}