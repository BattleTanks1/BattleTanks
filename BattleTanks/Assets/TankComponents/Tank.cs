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

    public float m_shootRange;
    [SerializeField]
    protected Rigidbody m_projectile = null;
    [SerializeField]
    private float m_projectileSpeed = 0.0f;

    public float m_timeBetweenShot;
    private float m_elaspedTime = 0.0f;

    public float m_scaredValue;
    public float m_maxValueAtPosition;
}