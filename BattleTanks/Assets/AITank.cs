using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://lecturer.ukdw.ac.id/~mahas/dossier/gameng_AIFG.pdf
//https://www.reddit.com/r/gamedev/comments/9onssu/where_can_i_learn_more_about_rts_ai/

public enum eAIState
{
    FindingEnemy = 0,
    MovingToSafety,
    Attack,
    SetDestinationToSafePosition,
    Idling
}

public class AITank : Tank
{
    [SerializeField]
    private eAIState m_currentState;
    [SerializeField]
    private int m_currentTargetID;

    // Start is called before the first frame update
    void Start()
    {
        m_currentTargetID = -1;
        m_ID = GameManager.Instance.addTank(this);
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_currentState)
        {
            case eAIState.FindingEnemy:
                {
                    foreach (AITank otherTank in GameManager.Instance.m_tanks)
                    {
                        if (m_ID != otherTank.m_ID &&
                            Vector3.Distance(transform.position, otherTank.transform.position) <= Mathf.Abs(m_minDistance))
                        {
                            m_currentTargetID = otherTank.m_ID;   
                            m_currentState = eAIState.Attack;
                            break;
                        }
                    }
                }
                break;
            case eAIState.SetDestinationToSafePosition:
                m_velocity = -m_velocity;
                m_currentState = eAIState.MovingToSafety;
                break;
            case eAIState.MovingToSafety:
                break;
            case eAIState.Attack:
                {
                    AITank target = GameManager.Instance.GetTank(m_currentTargetID);
                    if (target)
                    {
                        if (Vector3.Distance(target.transform.position, transform.position) <= Mathf.Abs(m_minDistance))
                        {
                            shoot();
                            m_currentState = eAIState.SetDestinationToSafePosition;
                        }
                    }
                }
                break;
            case eAIState.Idling:
                break;
        }
    }
}
