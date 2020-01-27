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
    MovingToSafety,
    Shoot,
    SetDestinationToSafePosition,
    Idling
}

public class AITank : Tank
{
    [SerializeField]
    private eAIState m_currentState;
    public Vector3 positionToMoveTo;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(m_faction == Faction.AIRed)
        {

        }
        else if(m_faction == Faction.AIRed)
        {

        }
        m_ID = fGameManager.Instance.addTank(this);

        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(m_faction == Faction.AIRed)
        {
            PlayerTank target = fGameManager.Instance.m_player;
            if (InfluenceMap.Instance.getValueOnPosition(target.transform.position) <= 3.0f)
            {
                if (target && Vector3.Distance(fGameManager.Instance.m_player.transform.position, transform.position) >= Mathf.Abs(0.001f))
                {
                    float step = m_speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                }
            }
            else
            {
                if (target && Vector3.Distance(fGameManager.Instance.m_player.transform.position, transform.position) >= Mathf.Abs(10.0f))
                {
                    float step = m_speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
                }
            }
        }
        else if(m_faction == Faction.AIBlue)
        {

        }
    }
}