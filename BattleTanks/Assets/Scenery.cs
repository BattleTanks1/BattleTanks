using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSceneryType
{
    None = 0,
    BlockEyeSight,
    ViewableOverTop
}

public class Scenery : MonoBehaviour
{
    public eSceneryType m_sceneryType;

    // Start is called before the first frame update
    void Start()
    {
        iRectangle rect = new iRectangle((int)transform.position.x - (int)(transform.localScale.x / 2.0f), 
            (int)transform.position.x + (int)(transform.localScale.x / 2.0f), 
            (int)transform.position.z - (int)(transform.localScale.z / 2.0f),
            (int)transform.position.z + (int)(transform.localScale.z / 2.0f));

        GameManager.Instance.addScenery(rect, m_sceneryType);
    }
}