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
    [SerializeField]
    private eFactionControllerType m_controllerType;
    [SerializeField]
    private int m_health = 1;

    public float m_scaredValue;
    public float m_maxValueAtPosition;

    private void Start()
    {
        m_ID = GameManager.Instance.addUnit();
    }

    public eFactionControllerType getControllerType()
    {
        return m_controllerType;
    }

    public void reduceHealth(int amount)
    {
        m_health -= amount;
    }

    public bool isDead()
    {
        return m_health <= 0;
    }
}