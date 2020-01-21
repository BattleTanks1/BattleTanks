﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://lecturer.ukdw.ac.id/~mahas/dossier/gameng_AIFG.pdf
//https://www.reddit.com/r/gamedev/comments/9onssu/where_can_i_learn_more_about_rts_ai/

public enum eAIState
{
    FindEnemy = 0,
    RunToSafety,
    Idle
}

public class TestScript : MonoBehaviour
{
    public Vector3 m_velocity;
    public float m_minDistance;
    public eAIState m_currentState;

    // Start is called before the first frame update
    void Start()
    {
    }

    void onFindEnemy()
    {
        GameObject[] gameObjectss = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject otherGameObject in gameObjectss)
        {
            print(otherGameObject.GetInstanceID());
        }

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Unit");
        foreach(GameObject otherGameObject in gameObjects)
        {
            if(this.gameObject.GetInstanceID() != otherGameObject.GetInstanceID() && 
                Vector3.Distance(this.transform.position, otherGameObject.transform.position) <= Mathf.Abs(m_minDistance))
            {   
                print(Vector3.Distance(this.transform.position, otherGameObject.transform.position));
                m_velocity = -m_velocity;
                m_currentState = eAIState.RunToSafety;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_currentState)
        {
            case eAIState.FindEnemy:
                onFindEnemy();
                break;
            case eAIState.RunToSafety:
                break;
            case eAIState.Idle:
                break;
        }

        transform.position += m_velocity * Time.deltaTime;
    }
}
