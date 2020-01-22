using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://lecturer.ukdw.ac.id/~mahas/dossier/gameng_AIFG.pdf
//https://www.reddit.com/r/gamedev/comments/9onssu/where_can_i_learn_more_about_rts_ai/

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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        m_ID = GameManager.Instance.addTank(this);
    }

    // Update is called once per frame
    protected override void Update() 
    {
        base.Update();

        switch(m_currentState)
        {
            case eAIState.FindingEnemy:
                if (isInRange(GameManager.Instance.m_player.transform.position))
                {
                    print("Shoot");
                    m_currentState = eAIState.Shoot;
                    
                }
                break;

            case eAIState.SetDestinationToSafePosition:
                m_velocity = -m_velocity;
                m_currentState = eAIState.MovingToSafety;
                break;

            case eAIState.MovingToSafety:
                break;

            case eAIState.Shoot:
                shoot();
                break;

            case eAIState.Idling:
                break;
        }

        transform.position += m_velocity * Time.deltaTime;
    }
}