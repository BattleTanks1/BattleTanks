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
    ShootAtEnemy,
    MovingToNewPosition,
    SetDestinationToSafePosition,
}

public class Tank : MonoBehaviour
{
    public float m_movementSpeed;
    public int m_visibilityDistance;

    public float m_threatStrength;
    public int m_threatDistance;

    public float m_proximityStrength;
    public int m_proximityDistance;

    public Vector3 m_oldPosition { get; protected set; }

    [SerializeField]
    public int m_ID { get; protected set; }

    [SerializeField]
    public eFactionName m_factionName;

    [SerializeField]
    protected Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;


    public float m_expiredTime;
    private float m_elaspedTime = 0.0f;

    //AI Stuff
    [SerializeField]
    public eAIState m_currentState;
    public eAIBehaviour m_behaviour;

    public Vector3 m_positionToMoveTo;
    public float m_scaredValue;
    public float m_maxValueAtPosition;
    public int m_targetID = Utilities.INVALID_ID;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_oldPosition = transform.position;
        fGameManager.Instance.updatePositionOnMap(this);
        m_ID = fGameManager.Instance.addTank(this);
    }

    protected virtual void Update()
    { 
        switch (m_currentState)
        {
            case eAIState.AwaitingDecision:

                break;
            case eAIState.FindEnemy:
                SearchRect searchRect = new SearchRect(Utilities.convertToGridPosition(transform.position), m_visibilityDistance);
                for (int y = searchRect.top; y <= searchRect.bottom; ++y)
                {
                    for (int x = searchRect.left; x <= searchRect.right; ++x)
                    {
                        if (Vector2Int.Distance(Utilities.convertToGridPosition(transform.position), new Vector2Int(x, y)) <= m_visibilityDistance &&
                            fGameManager.Instance.isEnemyOnPosition(new Vector2Int(x, y), m_factionName))
                        {
                            print("Spotted Enemy");
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
                if (!fGameManager.Instance.isPositionOccupied(newPosition, m_ID))
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
            case eAIState.ShootAtEnemy:
                {
                    bool targetFound = false;
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
                                targetFound = true;

                                //Shoot
                                Rigidbody clone;
                                clone = Instantiate(m_projectile, transform.position, Quaternion.identity);
                                Vector3 vBetween = new Vector3(x, 0, y) - transform.position;
                                clone.velocity = transform.TransformDirection(vBetween.normalized * 10);
                            }
                        }
                    }

                    if (!targetFound)
                    {
                        m_targetID = Utilities.INVALID_ID;
                        m_currentState = eAIState.AwaitingDecision;
                    }
                }
                break;
        }
    }
}