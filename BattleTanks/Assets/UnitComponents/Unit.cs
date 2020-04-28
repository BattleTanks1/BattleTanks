using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eUnitType
{
    Attacker = 0,
    Harvester
}

public class Unit : MonoBehaviour
{
    [SerializeField]
    private int m_visibilityDistance = 0;
    [SerializeField]
    private float m_threatStrength = 0.0f;
    [SerializeField]
    private int m_threatDistance = 0;
    [SerializeField]
    private float m_threatFallOffStrength = 0.0f;
    [SerializeField]
    private int m_threatFallOffDistance = 0;
    [SerializeField]
    private float m_proximityStrength = 0.0f;
    [SerializeField]
    private int m_proximityDistance = 0;
    [SerializeField]
    private int m_ID;
    [SerializeField]
    private eFactionName m_factionName;
    [SerializeField]
    private eFactionControllerType m_controllerType;
    [SerializeField]
    private eUnitType m_unitType;
    [SerializeField]
    private int m_health = 1;
    [SerializeField]
    private float m_scaredValue = 0.0f;

    private void Awake()
    {
        m_ID = Utilities.INVALID_ID;
    }

    private void Start()
    {
        m_ID = GameManager.Instance.addUnit();
    }

    public int getVisibilityDistance()
    {
        return m_visibilityDistance;
    }

    public int getID()
    {
        return m_ID;
    }

    public eUnitType getUnitType()
    {
        return m_unitType;
    }

    public eFactionControllerType getControllerType()
    {
        return m_controllerType;
    }

    public eFactionName getFactionName()
    {
        return m_factionName;
    }

    public float getScaredValue()
    {
        return m_scaredValue;
    }

    public void reduceHealth(int amount)
    {
        m_health -= amount;
    }

    public bool isDead()
    {
        return m_health <= 0;
    }

    public void createInfluence(FactionInfluenceMap[] proximityMaps, FactionInfluenceMap[] threatMaps)
    {
        Assert.IsNotNull(proximityMaps);
        Assert.IsNotNull(threatMaps);

        Vector2Int positionOnGrid = Utilities.convertToGridPosition(transform.position);
        proximityMaps[(int)m_factionName].createProximity(positionOnGrid, m_proximityStrength, m_proximityDistance);
        threatMaps[(int)m_factionName].createThreat(positionOnGrid, m_threatStrength, m_threatDistance,
            m_threatFallOffStrength, m_threatFallOffDistance);
    }
}