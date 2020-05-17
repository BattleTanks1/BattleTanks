using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidResolver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_velocity += m_acceleration * m_maxAcceleration * Time.deltaTime;
        //Temp code capping velocity in place of drag
        float magnitude = std::min(m_velocity.magnitude, 5.0f);
        m_velocity = m_velocity.normalized * magnitude;

        m_position += m_velocity * Time.deltaTime;
    }
}
