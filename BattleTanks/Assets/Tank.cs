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
    
    //Test States
    Idle,
    Flee
}

public class Tank : MonoBehaviour
{
    public float m_movementSpeed;
    public int m_visibilityDistance;

    public float m_threatStrength;
    public int m_threatDistance;
    public float m_threatFallOffStrength;
    public int m_threatFallOffDistance;

    public float m_proximityStrength;
    public int m_proximityDistance;

    public Vector3 m_oldPosition { get; protected set; }

    public int m_ID;

    [SerializeField]
    public eFactionName m_factionName;

    public float m_shootRange;
    [SerializeField]
    protected Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;

    public float m_timeBetweenShot;
    private float m_elaspedTime = 0.0f;

    //AI Stuff
    [SerializeField]
    public eAIState m_currentState;

    public Vector3 m_positionToMoveTo;
    public float m_scaredValue;
    public float m_maxValueAtPosition;
    public int m_targetID = Utilities.INVALID_ID;
    public Vector3 velocity;

    // Start is called before the first frame update
    private void Start()
    {
        m_oldPosition = transform.position;
        m_ID = fGameManager.Instance.addTank(this);
        fGameManager.Instance.updatePositionOnMap(this);
        m_elaspedTime = m_timeBetweenShot;


    }

    private void move()
    {
        //Moving to enemy position - 'm_positionToMoveTo
        if (m_currentState == eAIState.TargetEnemy && 
            Vector3.Distance(m_positionToMoveTo, transform.position) > m_shootRange)
        {
            float step = m_movementSpeed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, m_positionToMoveTo, step);
            if (!fGameManager.Instance.isPositionOccupied(newPosition, m_ID))
            {
                m_oldPosition = transform.position;
                transform.position = newPosition;
                fGameManager.Instance.updatePositionOnMap(this);
            }
            else
            {
                print("Occupied");
            }
        }
        else if(m_currentState == eAIState.Flee)
        {
            Vector3 newPosition = transform.position + velocity * Time.deltaTime;
            if (!fGameManager.Instance.isPositionOccupied(newPosition, m_ID))
            {
                m_oldPosition = transform.position;
                transform.position = newPosition;
                fGameManager.Instance.updatePositionOnMap(this);
            }
        }
    }

    private void Update()
    {
        m_elaspedTime += Time.deltaTime;
        move();

        switch (m_currentState)
        {
            case eAIState.AwaitingDecision:

                break;
            case eAIState.FindEnemy:
                Rectangle searchRect = new Rectangle(Utilities.convertToGridPosition(transform.position), m_visibilityDistance);
                for (int y = searchRect.m_top; y <= searchRect.m_bottom; ++y)
                {
                    for (int x = searchRect.m_left; x <= searchRect.m_right; ++x)
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
                //m_positionToMoveTo = PathFinding.Instance.getClosestSafePosition(transform.position, 8);
                m_currentState = eAIState.MovingToNewPosition;

                break;

            case eAIState.MovingToNewPosition:
                {
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
                }
                break;
            case eAIState.TargetEnemy:
                {
                    Vector3 enemyPosition = new Vector3();
                    Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
                    Rectangle searchableRect = new Rectangle(positionOnGrid, m_visibilityDistance);

                    for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
                    {
                        for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
                        {
                            float distance = Vector2Int.Distance(positionOnGrid, new Vector2Int(x, y));
                            if (distance <= m_visibilityDistance &&
                                fGameManager.Instance.getPointOnMap(y, x).tankID == m_targetID)
                            {
                                enemyPosition = new Vector3(x, 0, y);

                                break;
                            }
                        }
                    }

                    if (m_elaspedTime >= m_timeBetweenShot &&
                        Vector3.Distance(enemyPosition, transform.position) <= m_shootRange)
                    {
                        m_elaspedTime = 0.0f;

                        Rigidbody clone;
                        clone = Instantiate(m_projectile, transform.position, Quaternion.identity);
                        Vector3 vBetween = enemyPosition - transform.position;
                        clone.velocity = transform.TransformDirection(vBetween.normalized * m_projectileSpeed);
                    }
                }
                break;
        }
    }
}