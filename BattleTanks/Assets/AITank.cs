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
    FindingEnemy = 0,
    MovingToNewPosition,
    Shoot,
    SetDestinationToSafePosition,
    Idling
}

public class AITank : Tank
{
    [SerializeField]
    public eAIState m_currentState;
    public Vector3 m_positionToMoveTo;
    public float m_scaredValue;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_ID = fGameManager.Instance.addAITank(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(m_faction == Faction.AIRed)
        {
            AITank target = fGameManager.Instance.getBlueTank();
            if (target && InfluenceMap.Instance.getPointOnProximityMap(target.transform.position).value <= 3.0f)
            {
                if (target && Vector3.Distance(target.transform.position, transform.position) >= Mathf.Abs(0.001f))
                {
                    float step = m_speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                }
            }
            else
            {
                if (target && Vector3.Distance(target.transform.position, transform.position) >= Mathf.Abs(10.0f))
                {
                    float step = m_speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                }
            }
        }
        else if(m_faction == Faction.AIBlue)
        {
            //Idling
            switch (m_currentState)
            {
                case eAIState.Idling:
                    if (InfluenceMap.Instance.isPositionInThreat(this))
                    {
                        m_currentState = eAIState.SetDestinationToSafePosition;
                    }
                    break;
                case eAIState.SetDestinationToSafePosition:
                    m_positionToMoveTo = PathFinding.Instance.getClosestSafePosition(transform.position);
                    m_currentState = eAIState.MovingToNewPosition;

                    break;

                case eAIState.MovingToNewPosition:
                    if(transform.position == m_positionToMoveTo)
                    {
                        m_currentState = eAIState.Idling;
                    }
                    break;
            }
        }
    }
}