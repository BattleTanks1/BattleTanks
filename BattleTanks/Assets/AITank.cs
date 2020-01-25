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
    bool reachedTarget = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_ID = fGameManager.Instance.addTank(this);
        m_faction = Faction.AI;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //switch(m_currentState)
        //{
        //    case eAIState.FindingEnemy:
        //        Tank target = fGameManager.Instance.GetTank(Faction.player);
        //        if(target && isInRange(target.transform.position))
        //        {
        //            m_currentState = eAIState.Shoot;
        //        }
        //        break;

        //    case eAIState.SetDestinationToSafePosition:
        //        m_movementSpeed = -m_movementSpeed;
        //        m_currentState = eAIState.MovingToSafety;
        //        break;

        //    case eAIState.MovingToSafety:
        //        break;

        //    case eAIState.Shoot:
        //        shoot();
        //        m_currentState = eAIState.FindingEnemy;
        //        break;

        //    case eAIState.Idling:
        //        break;
        //}


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
            print("Hit");
            if (target && Vector3.Distance(fGameManager.Instance.m_player.transform.position, transform.position) >= Mathf.Abs(10.0f))
            {
                float step = m_speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
            }
        }
    }
}