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

public enum eAIState
{
    AwaitingDecision = 0,
    FindEnemy,
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
                            //Send message to commander
                            m_currentState = eAIState.AwaitingDecision;
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
        }
    }
}