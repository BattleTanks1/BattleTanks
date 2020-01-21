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

public class Tank : MonoBehaviour
{
    public int m_health { get; private set; }
    public GameObject m_projectileSpawn;
    public GameObject m_projectile;
    [SerializeField]
    private Vector3 m_velocity;
    public Vector3 Velocity { get { return m_velocity; } private set { m_velocity = Velocity; } }
    [SerializeField]
    public float m_minDistance { get; private set; }
    [SerializeField]
    public eAIState m_currentState;
    [SerializeField]
    private int m_currentTargetID;
    [SerializeField]
    public int m_ID { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        m_health = 5;
        m_minDistance = 3;
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
                    foreach (Tank otherTank in GameManager.Instance.m_tanks)
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
                    Tank target = GameManager.Instance.GetTank(m_currentTargetID);
                    if (target)
                    {
                        if (Vector3.Distance(target.transform.position, transform.position) <= Mathf.Abs(m_minDistance))
                        {
                            GameObject projectile;
                            projectile = Instantiate(m_projectile, m_projectileSpawn.transform.position, m_projectile.transform.rotation);
                            m_currentState = eAIState.SetDestinationToSafePosition;
                        }
                    }
                }
                break;
            case eAIState.Idling:
                break;
        }

        transform.position += m_velocity * Time.deltaTime;
    }

    public void damage(int amount)
    {
        m_health -= amount;
    }
}
