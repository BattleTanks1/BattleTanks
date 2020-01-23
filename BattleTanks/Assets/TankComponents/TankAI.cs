using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://lecturer.ukdw.ac.id/~mahas/dossier/gameng_AIFG.pdf
//https://www.reddit.com/r/gamedev/comments/9onssu/where_can_i_learn_more_about_rts_ai/

public enum AIState
{
    FindingEnemy = 0,
    MovingToSafety,
    Shoot,
    SetDestinationToSafePosition,
    Idling
}

public class TankAI : MonoBehaviour
{
    [SerializeField]
    private AIState m_currentState;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TankCore>().m_faction = Faction.AI;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_currentState)
        {
            case AIState.FindingEnemy:
                //if (isInRange(GameManager.Instance.m_player.transform.position))
                //{
                //    m_currentState = eAIState.Shoot;
                //}
                break;

            case AIState.SetDestinationToSafePosition:
                m_currentState = AIState.MovingToSafety;
                break;

            case AIState.MovingToSafety:
                break;

            case AIState.Shoot:
                m_currentState = AIState.FindingEnemy;
                break;

            case AIState.Idling:
                break;
        }
    }
}