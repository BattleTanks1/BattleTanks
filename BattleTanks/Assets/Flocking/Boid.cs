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
    private BoidBox m_parent = null;
    private int m_index = 0;
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
    Vector3 accumulate(Vector3 acc, Vector3 add)
    {
        if (acc.magnitude == 1.0f)
            return acc;

        float dot = Vector3.Dot(acc, add);
        float root = Mathf.Sqrt((dot * dot) - (add.sqrMagnitude * (acc.sqrMagnitude - 1)));
        float A = (-dot + root) / (acc.sqrMagnitude - 1);
        float B = (-dot - root) / (acc.sqrMagnitude - 1);

        if (A >= 0.0f)
        {
            A = Mathf.Min(1.0f, A);
            acc = acc + (add * A);
            acc = acc.normalized;
            return acc;
        }
        else if (B >= 0.0f)
        {
            B = Mathf.Min(1.0f, B);
            acc = acc + (add * B);
            acc = acc.normalized;
            return acc;
        }
        return acc;
    }

    Vector3 collisionAvoidance(BoidTracker[] boids, Vector3 pos, float avoidDist)
    {
        Vector3 sum = Vector3.zero;
        foreach (BoidTracker boid in boids)
        {
            Vector3 diff = pos - boid.m_boid.m_position;
            if (diff.magnitude < avoidDist)
            {
                sum += diff - (diff / avoidDist);
            }
        }
        if (sum.magnitude > 1)
            return sum.normalized;
        else
            return sum;
    }

    void Update()
    {
        Vector3 oldAcc = m_acceleration;
        //Find "flock" data
        BoidTracker[] boids = m_parent.getBoids();//TEMP!! Inefficient to call each frame
        Vector3 sumPos = Vector3.zero;
        Vector3 sumVel = Vector3.zero;
        foreach (BoidTracker boid in boids)
        {
            //rather than creating the flock store the average of nearby velocities and positions simultaneously as it saves on temp data
            Vector3 diff = m_position - boid.m_boid.m_position;
            if (diff.magnitude < m_detectionDistance)
            {
                if (diff == Vector3.zero)
                    continue;
                if (m_velocity != Vector3.zero)
                {
                    float sigma = Vector3.Dot(m_velocity, diff) / (diff.magnitude * m_velocity.magnitude);
                    if (Mathf.Acos(sigma) > Mathf.PI * 0.75)
                        continue;
                }
                sumPos += boid.m_boid.m_position;
                sumVel += boid.m_boid.m_velocity;
            }
        }

        //Collision avoidance
        m_acceleration = collisionAvoidance(boids, m_position, m_avoidanceDistance);

        //Home towards 0, 0
        Vector3 homeVec = m_homePos - m_position;
        if (homeVec.magnitude > m_homeBounds)
        {
            float mult = (homeVec.magnitude - m_homeBounds) / m_homeBounds;
            m_acceleration = accumulate(m_acceleration, homeVec.normalized * mult);
        }

        //Match velocity with flock
        if (sumVel != Vector3.zero)
        {
            if (sumVel.magnitude > 1.0f)
                sumVel = sumVel.normalized;
            Vector3 matchVel = (sumVel - m_velocity) * m_maxAcceleration;
            m_acceleration = accumulate(m_acceleration, matchVel);
        }

        //Move toward flock centre
        if (sumPos != Vector3.zero)
        {
            if (sumPos.magnitude > 1.0f)
                sumPos = sumPos.normalized;
            Vector3 matchPos = sumPos / m_detectionDistance;
            m_acceleration = accumulate(m_acceleration, matchPos);
        }

        //Random acceleration

        //Continue on current path
        m_acceleration = accumulate(m_acceleration, m_velocity);

        //Damping
        m_acceleration = (m_acceleration + oldAcc) / 2;

        //Actually interacting with stuff
        m_velocity += m_acceleration * m_maxAcceleration * Time.deltaTime;

        float drag = m_velocity.sqrMagnitude * m_dragEffect;
        m_velocity -= m_velocity * drag * Time.deltaTime;

        m_position += m_velocity * Time.deltaTime;
        transform.position = m_position;

        //Rotate towards the new velocity
        transform.rotation = Quaternion.LookRotation(m_velocity, Vector3.up);
    }

    public void setParent(in BoidBox parent, int index)
    {
        m_parent = parent;
        m_index = index;
    }

    //Death behaviour TODO
}