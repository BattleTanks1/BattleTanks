using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    EnemySpottedAtPosition = 0
}

public class MessageToAIController
{
    public MessageToAIController(Vector2Int position, eAIUniMessageType messageType, int senderID, eFactionName senderFaction)
    {
        m_position = position;
        m_messageType = messageType;
        m_senderID = senderID;
        m_senderFaction = senderFaction;
        m_targetID = Utilities.INVALID_ID;
    }

    public MessageToAIController(int targetID, Vector2Int position, eAIUniMessageType messageType, int senderID, eFactionName senderFaction)
    {
        m_targetID = targetID;
        m_position = position;
        m_messageType = messageType;
        m_senderID = senderID;
        m_senderFaction = senderFaction;
    }

    public Vector2Int m_position { get; private set; }
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
    Shoot,
    SetDestinationToSafePosition,
}

public class AITank : Tank
{
    [SerializeField]
    public eAIState m_currentState;
    public eAIBehaviour m_behaviour;

    public Vector3 m_positionToMoveTo;
    public float m_scaredValue;
    public float m_maxValueAtPosition;
    public int m_targetID = Utilities.INVALID_ID;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_ID = fGameManager.Instance.addTank(this);
        m_currentState = eAIState.AwaitingDecision;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //Idling
        switch (m_currentState)
        {
            case eAIState.AwaitingDecision:

                break;
            case eAIState.FindEnemy:
                SearchRect searchRect = new SearchRect(Utilities.convertToGridPosition(transform.position), m_visibilityDistance);
                for(int y = searchRect.top; y <= searchRect.bottom; ++y)
                {
                    for (int x = searchRect.left; x <= searchRect.right; ++x)
                    {
                        if (Vector2Int.Distance(Utilities.convertToGridPosition(transform.position), new Vector2Int(x, y)) <= m_visibilityDistance &&
                            fGameManager.Instance.isEnemyOnPosition(new Vector2Int(x, y), m_factionName))
                        {
                            m_currentState = eAIState.AwaitingDecision;

                            //Send message to commander
                            GraphPoint pointOnEnemy = fGameManager.Instance.getPointOnMap(new Vector2Int(x, y));
                            MessageToAIController message = new MessageToAIController(pointOnEnemy.tankID, new Vector2Int(x, y), eAIUniMessageType.EnemySpottedAtPosition,
                                m_ID, m_factionName);
                            fGameManager.Instance.sendAIControllerMessage(message);
                        }
                    }
                }
                break;
            case eAIState.SetDestinationToSafePosition:
                m_positionToMoveTo = PathFinding.Instance.getClosestSafePosition(transform.position, 8);
                m_currentState = eAIState.MovingToNewPosition;

                break;

            case eAIState.MovingToNewPosition:
                float step = m_movementSpeed * Time.deltaTime;
                Vector3 newPosition = Vector3.MoveTowards(transform.position, m_positionToMoveTo, step);
                if(!fGameManager.Instance.isPositionOccupied(newPosition, m_ID))
                {
                    m_oldPosition = transform.position;
                    transform.position = newPosition;
                    fGameManager.Instance.updatePositionOnMap(this);
                    if (transform.position == m_positionToMoveTo)
                    {
                        m_currentState = eAIState.AwaitingDecision;
                    }
                }
                break;
            case eAIState.TargetEnemy:
                {
                    if(isTargetStillInSight(m_targetID))
                    {
                        //Shoot
                    }
                    else
                    {
                        m_currentState = eAIState.AwaitingDecision;
                    }
                }
                break;
        }
    }

    private bool isTargetStillInSight(int targetID)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        SearchRect searchableRect = new SearchRect(positionOnGrid, m_visibilityDistance);
        for (int y = searchableRect.top; y <= searchableRect.bottom; ++y)
        {
            for (int x = searchableRect.left; x <= searchableRect.right; ++x)
            {
                float distance = Vector2Int.Distance(positionOnGrid, new Vector2Int(x, y));
                if (distance <= m_visibilityDistance &&
                    fGameManager.Instance.getPointOnMap(y, x).tankID == m_targetID)
                {
                    return true;
                }
            }
        }

        return false;
    }
}