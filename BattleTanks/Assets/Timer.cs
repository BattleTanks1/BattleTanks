using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float m_expiredTime { get; set; }
    private float m_elaspedTime = 0.0f;

    public bool m_active { get; set; }

    public void update(float deltaTime)
    {
        if (m_active)
        {
            m_elaspedTime += deltaTime;
        }
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