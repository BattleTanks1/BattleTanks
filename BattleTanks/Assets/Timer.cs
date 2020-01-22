using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    [SerializeField]
    private float m_expiredTime;
    private float m_elaspedTime;

    public bool m_active { get; set; }

    public void update(float deltaTime)
    {
        if (m_active)
        {
            m_elaspedTime += deltaTime;
        }
    }

    public void stop()
    {
        m_active = false;
        m_elaspedTime = 0.0f;
    }

    public void reset()
    {
        m_elaspedTime = 0.0f;
    }

    public bool isExpired()
    {
        return m_elaspedTime >= m_expiredTime;
    }
}