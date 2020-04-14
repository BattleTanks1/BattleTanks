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
    FindEnemy,
    TargetEnemy,
    MovingToNewPosition,
    SetDestinationToSafePosition,
    FleeToSafeLocation,
    FleeToClosestSafeAlly,

    //Test States
    Idle,
    Flee
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
        switch (m_currentState)
        {
            case eAIState.AwaitingDecision:
                break;
            case eAIState.FindEnemy:
                {
                    iRectangle searchRect = new iRectangle(Utilities.convertToGridPosition(transform.position), m_tank.m_visibilityDistance);
                    for (int y = searchRect.m_top; y <= searchRect.m_bottom; ++y)
                    {
                        for (int x = searchRect.m_left; x <= searchRect.m_right; ++x)
                        {
                            Vector2Int positionOnGrid = new Vector2Int(x, y);
                            if (Vector2Int.Distance(Utilities.convertToGridPosition(transform.position), positionOnGrid) <= m_tank.m_visibilityDistance &&
                                GameManager.Instance.isEnemyOnPosition(positionOnGrid, m_tank.m_factionName))
                            {
                                print("Spotted Enemy");
                                m_currentState = eAIState.AwaitingDecision;

                                SendMessageToCommander(positionOnGrid, eAIUniMessageType.EnemySpottedAtPosition);
                            }
                        }
                    }
                }
              
                break;
            case eAIState.SetDestinationToSafePosition:
                {
                    m_tankMovement.moveTo(PathFinding.Instance.getClosestSafePosition(8, m_tank));
                    m_currentState = eAIState.MovingToNewPosition;                    
                }
                break;
            case eAIState.MovingToNewPosition:
                {
                    if(m_tankMovement.reachedDestination())
                    {
                        m_currentState = eAIState.AwaitingDecision;
                    }
                }
                break;
            case eAIState.TargetEnemy:
                {
                    bool targetSpotted = false;
                    Vector3 enemyPosition = new Vector3();
                    Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
                    iRectangle searchableRect = new iRectangle(positionOnGrid, m_tank.m_visibilityDistance);

                    for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
                    {
                        for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
                        {
                            float distance = Vector2Int.Distance(positionOnGrid, new Vector2Int(x, y));
                            if (distance <= m_tank.m_visibilityDistance &&
                                GameManager.Instance.getPointOnMap(y, x).tankID == m_targetID)
                            {
                                enemyPosition = new Vector3(x, 0, y);
                                targetSpotted = true;
                                break;
                            }
                        }
                    }

                    if (targetSpotted)
                    {
                        TankShooting tankShooting = GetComponent<TankShooting>();
                        Assert.IsNotNull(tankShooting);
                        if (tankShooting)
                        {
                            tankShooting.FireAtPosition(enemyPosition);
                        }
                    }
                }
                break;
        }
    }

    private void SendMessageToCommander(Vector2Int positionOnGrid, eAIUniMessageType messageType)
    {
        GraphPoint pointOnEnemy = GameManager.Instance.getPointOnMap(positionOnGrid);
        MessageToAIController message = new MessageToAIController(pointOnEnemy.tankID, positionOnGrid, eAIUniMessageType.EnemySpottedAtPosition,
            m_tank.m_ID, m_tank.m_factionName);
        
        GameManager.Instance.sendAIControllerMessage(message);
    }
}
