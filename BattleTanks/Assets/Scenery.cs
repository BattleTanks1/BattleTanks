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
    [SerializeField]
    private eSceneryType m_sceneryType;

    void Start()
    {
        GameManager.Instance.addScenery(new iRectangle(transform.position, transform.localScale), m_sceneryType);
    }
}