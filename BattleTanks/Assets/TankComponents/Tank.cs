using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    public int m_visibilityDistance;
    public float m_threatStrength;
    public int m_threatDistance;
    public float m_threatFallOffStrength;
    public int m_threatFallOffDistance;
    public float m_proximityStrength;
    public int m_proximityDistance;

    public int m_ID;

    [SerializeField]
    public eFactionName m_factionName;



    public float m_scaredValue;
    public float m_maxValueAtPosition;

    private void Start()
    {
        m_ID = GameManager.Instance.addTank(this);
    }
}