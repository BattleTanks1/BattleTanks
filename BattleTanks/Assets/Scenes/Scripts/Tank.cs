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
    Idling
}

public class Tank : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_velocity;
    public Vector3 Velocity { get { return m_velocity; } private set { m_velocity = Velocity; } }
    [SerializeField]
    public float m_minDistance { get; private set; }
    [SerializeField]
    public eAIState m_currentState { get; private set; }
    [SerializeField]
    private int m_currentTargetID;
    [SerializeField]
    public int m_ID { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        m_currentTargetID = -1;
        m_ID = GameManager.Instance.addTank(this);
    }

    void onFindEnemy()
    {
        foreach(Tank otherTank in GameManager.Instance.m_tanks)
        {
            if(m_ID != otherTank.m_ID &&
                Vector3.Distance(transform.position, otherTank.transform.position) <= Mathf.Abs(m_minDistance))
            {
                m_velocity = -m_velocity;
                m_currentState = eAIState.MovingToSafety;
            }
        }
    }

    void onAttack()
    {
        Tank target = GameManager.Instance.GetTank(m_currentTargetID);
        if(target)
        {
            if(Vector3.Distance(target.transform.position, transform.position) <= Mathf.Abs(m_minDistance))
            {
              
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_currentState)
        {
            case eAIState.FindingEnemy:
                onFindEnemy();
                break;
            case eAIState.MovingToSafety:
                break;
            case eAIState.Attack:
                onAttack();
                break;
            case eAIState.Idling:
                break;
        }

        transform.position += m_velocity * Time.deltaTime;
    }
}
