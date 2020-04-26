using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_position;
    [SerializeField]
    private Vector3 m_velocity;
    [SerializeField]
    private Vector3 m_acceleration = Vector3.zero;
    public Vector3 m_homePos = Vector3.zero;
    public float m_homeBounds = 10.0f;
    public float m_maxAcceleration = 10.0f;
    public float m_dragEffect = 0.0f;
    public float m_avoidanceDistance = 5.0f;
    public float m_detectionDistance = 5.0f;

    void Awake()
    {
        m_position = transform.position;
        m_velocity = new Vector3(
            Random.Range(-10.0f, 10.0f),
            Random.Range(-10.0f, 10.0f),
            Random.Range(-10.0f, 10.0f));
    }

    //Adds a vector to another, capping the length of the second such that the result's length is 1 or less
    void accumulate(Vector3 acc, Vector3 add)
    {
        if (acc.magnitude == 1.0f)
            return;

        float dot = Vector3.Dot(acc, add);
        float root = Mathf.Sqrt((dot * dot) - (add.sqrMagnitude * (acc.sqrMagnitude - 1)));
        float A = (-dot + root) / (acc.sqrMagnitude - 1);
        float B = (-dot - root) / (acc.sqrMagnitude - 1);

        if (A >= 0.0f)
        {
            A = Mathf.Min(1.0f, A);
            acc = acc + (add * A);
            acc = acc.normalized;
            return;
        }
        else if (B >= 0.0f)
        {
            B = Mathf.Min(1.0f, B);
            acc = acc + (add * B);
            acc = acc.normalized;
            return;
        }
    }

    //Vector3 collisionAvoidance(std::vector<Vector3> obstacles, Vector3 pos, float avoidDist)
    //{
    //    Vector3 sum = Vector3.zero;
    //    for (Vector3 obj : obstacles)
    //    {
    //        Vector3 diff = obj - pos;
    //        if (diff.square() < avoidDist)
    //        {
    //            sum = sum + diff.normalized;
    //        }
    //    }
    //    if (sum.mag() > 1)
    //        return sum.normalized;
    //    else
    //        return sum;
    //}

    void update()
    {
        //Find "flock" data
        Boid[] boids = GameObject.FindObjectsOfType<Boid>();//TEMP!! Inefficient to call each frame
        Vector3 sumPos = Vector3.zero;
        Vector3 sumVel = Vector3.zero;
        foreach (Boid boid in boids)
        {
            //rather than creating the flock store the average of nearby velocities and positions simultaneously as it saves on temp data
            Vector3 diff = m_position - boid.m_position;
            if (diff.sqrMagnitude < m_detectionDistance)
            {
                if (diff == Vector3.zero)
                    continue;

                sumPos += boid.m_position;
                sumVel += boid.m_velocity;
            }
        }
        sumPos = sumPos.normalized;

        //Collision avoidance
        Vector3 collision = Vector3.zero;//collisionAvoidance(obstacles, m_position, m_avoidanceDistance);

        //Move toward home point
        Vector3 relativePos = m_position - m_homePos;
        Vector3 stayHome = Vector3.zero;
        if (relativePos.magnitude > m_homeBounds)
        {
            float intensity = (relativePos.magnitude - m_homeBounds) / m_homeBounds;
            stayHome = -relativePos * intensity;
        }

        //Match velocity with flock
        sumVel = sumVel.normalized;
        Vector3 matchVel = (sumVel - m_velocity) / m_maxAcceleration;

        //Move toward flock centre
        sumPos = sumPos.normalized;
        Vector3 matchPos = (sumPos - m_position) / m_detectionDistance;

        //Random acceleration
        Vector3 randmotion = new Vector3(
            Random.Range(-10.0f, 10.0f),
            Random.Range(-10.0f, 10.0f),
            Random.Range(-10.0f, 10.0f));
        randmotion = randmotion.normalized;

        //Accumulate
        m_acceleration = collision;
        accumulate(m_acceleration, stayHome);
        accumulate(m_acceleration, matchVel);
        accumulate(m_acceleration, matchPos);
        accumulate(m_acceleration, randmotion);

        //Actually interacting with normalizedy
        m_velocity += m_acceleration * m_maxAcceleration * Time.deltaTime;
        //Temp code capping velocity in place of drag
        float magnitude = Mathf.Min(m_velocity.Magnitude, 5.0f);
        m_velocity = m_velocity.normalized * magnitude;

        m_position += m_velocity * Time.deltaTime;
    }

    //void simulate(float Time.deltaTime)
    //{
    //    m_velocity += m_acceleration * m_maxAcceleration * Time.deltaTime;
    //    //Temp code capping velocity in place of drag
    //    float magnitude = std::min(m_velocity.mag(), 5.0f);
    //    m_velocity = m_velocity.normalized * magnitude;

    //    m_position += m_velocity * Time.deltaTime;
    //}
}