using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField]
    private bool m_testing = false;
    [SerializeField]
    private Vector3 m_position;
    [SerializeField]
    private Vector3 m_velocity;
    //[SerializeField]
    //private float m_acceleration = Vector3.zero;

    private BoidSpawner m_parent = null;
    private int m_index = 0;
    private Vector3 m_homePos = Vector3.zero;

    //Tuning variables
    [SerializeField]
    private float m_homeBounds = 10.0f;
    [SerializeField]
    private float m_maxAcceleration = 3.0f;
    [SerializeField]
    private float m_speed = 5.0f;
    [SerializeField]
    private float m_avoidanceDistance = 3.0f;
    [SerializeField]
    private float m_detectionDistance = 10.0f;
    [SerializeField]
    private float m_viewAngle = 0.75f;

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
            return acc.normalized;

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

    Vector3 collisionAvoidance(BoidTracker[] boids, Vector3 perpNormal)
    {
        
        Vector2Int roundedPosition = Utilities.convertToGridPosition(m_position);
        Vector3 oobAvoid = Vector3.zero;
        //OOB avoidance
        if (roundedPosition.x < 3 || roundedPosition.y < 3 || roundedPosition.x > 247 || roundedPosition.y > 247)
        {
            Vector3 centreOfMap = new Vector3(125.0f, 1.0f, 125.0f);

            Vector3 diff = centreOfMap - m_position;

            float dot = Vector3.Dot(diff, perpNormal);
            //Induce hard turn
            if (dot > 0.0f)
                oobAvoid += perpNormal;
            else
                oobAvoid -= perpNormal;

        }
        Vector3 scenerySum = Vector3.zero;
        //Scenery avoidance
        //Find objects that are too close
        int bound = (int)Mathf.Ceil(m_avoidanceDistance);
        for (int x = -bound; x <= bound; ++x)
        {
            for (int y = -bound; y <= bound; ++y)
            {
                if (!Map.Instance.isInBounds(roundedPosition.x + x, roundedPosition.y + y) || 
                    Map.Instance.getPoint(roundedPosition.x + x, roundedPosition.y + y).scenery)
                {
                    Vector3 obstPosition = new Vector3(roundedPosition.x + x, 1.0f, roundedPosition.y + y);

                    Vector3 diff = m_position - obstPosition;
                    if (diff.sqrMagnitude < m_avoidanceDistance * m_avoidanceDistance)
                    {
                        float dot = Vector3.Dot(diff, perpNormal);
                        //Induce hard turns from scenery
                        if (dot > 0.0f)
                            scenerySum += perpNormal * m_avoidanceDistance / (diff.magnitude + 0.01f);
                        else
                            scenerySum -= perpNormal * m_avoidanceDistance / (diff.magnitude + 0.01f);
                    }
                }
            }
        }

        Vector3 boidSum = Vector3.zero;
        //Boid avoidance
        foreach (BoidTracker boid in boids)
        {
            if (boid.m_boid == null)
                continue;

            Vector3 diff = m_position - boid.m_boid.m_position;
            if (diff.sqrMagnitude < m_avoidanceDistance * m_avoidanceDistance)
            {
                boidSum += diff.normalized * m_avoidanceDistance / (diff.sqrMagnitude + 0.01f);
            }
        }
        Vector3 finalSum = accumulate(oobAvoid, scenerySum);
        finalSum = accumulate(finalSum, boidSum);

        if (m_testing)
        {
            Debug.Log("Sum of avoidance");
            Debug.Log(finalSum);
        }
        return finalSum;
    }

    void Update()
    {
        //Vector3 oldAcc = m_acceleration;
        Vector3 acceleration = Vector3.zero;
        //Get perpendicular normal vector for velocity
        float x = m_velocity.x * Mathf.Cos(Mathf.PI * 0.5f) + m_velocity.z * Mathf.Sin(Mathf.PI * 0.5f);
        float z = m_velocity.z * Mathf.Cos(Mathf.PI * 0.5f) - m_velocity.x * Mathf.Sin(Mathf.PI * 0.5f);
        Vector3 perpVec = new Vector3(x, 0.0f, z).normalized;
        //Find "flock" data
        BoidTracker[] boids = m_parent.getBoids();
        Vector3 sumPos = Vector3.zero;
        Vector3 sumVel = Vector3.zero;
        int numBoids = 0;
        foreach (BoidTracker boid in boids)
        {
            if (boid.m_boid == null)
                continue;
            
            Vector3 diff = m_position - boid.m_boid.m_position;
            if (diff.sqrMagnitude < m_detectionDistance * m_detectionDistance)
            {
                if (diff == Vector3.zero)
                    continue;
                if (m_velocity != Vector3.zero)
                {
                    float sigma = Vector3.Dot(m_velocity, diff) / (diff.magnitude * m_velocity.magnitude);
                    if (Mathf.Acos(sigma) > Mathf.PI * m_viewAngle)
                        continue;
                }
                sumPos += boid.m_boid.m_position;
                sumVel += boid.m_boid.m_velocity;
                ++numBoids;
            }
        }
        if (m_testing)
        {
            Debug.Log("Position and velocity sums");
            Debug.Log(sumPos);
            Debug.Log(sumVel);
        }

        //Collision avoidance
        acceleration = collisionAvoidance(boids, perpVec);

        //Home towards 0, 0
        Vector3 homeVec = m_homePos - m_position;
        if (homeVec.sqrMagnitude > m_homeBounds * m_homeBounds)
        {
            float mult = (homeVec.magnitude - m_homeBounds) / (m_homeBounds * m_maxAcceleration);
            acceleration = accumulate(acceleration, homeVec.normalized * mult);
        }

        //Match velocity with flock
        if (sumVel != Vector3.zero)
        {
            Vector3 matchVel = (sumVel / numBoids) - m_velocity;
            if (m_testing)
            {
                Debug.Log("Velocity matching vector");
                Debug.Log(matchVel);
            }
            acceleration = accumulate(acceleration, matchVel);
        }

        //Move toward flock centre
        if (sumPos != Vector3.zero)
        {
            Vector3 matchPos = (sumPos / numBoids) - m_position;
            if (m_testing)
            {
                Debug.Log("Position seeking vector");
                Debug.Log(matchPos);
            }
            acceleration = accumulate(acceleration, matchPos);
        }

        //Random acceleration?

        //Damping
        //acceleration = (acceleration + oldAcc) / 2;

        //Get dot product of acc with perp normal vector
        if (m_testing)
        {
            Debug.Log("Perpendicular vector");
            Debug.Log(perpVec);
            Debug.Log("Incoming accelerations");
            Debug.Log(acceleration);
        }
        acceleration = perpVec * Vector3.Dot(acceleration, perpVec) * m_maxAcceleration;
        //Apply steering to velocity and velocity to position
        if (m_testing)
            Debug.Log(acceleration);
        if (acceleration == Vector3.zero)
            acceleration += perpVec * 0.1f;
        Vector3 newVelocity = m_velocity + acceleration * Time.deltaTime;
        m_velocity = newVelocity.normalized * m_speed;
        m_velocity = new Vector3(m_velocity.x, 0.0f, m_velocity.z);
        m_position += m_velocity * Time.deltaTime;
        transform.position = m_position;

        //Rotate towards the new velocity
        transform.rotation = Quaternion.LookRotation(m_velocity, Vector3.up);
    }

    public void setParent(in BoidSpawner parent, int index)
    {
        m_parent = parent;
        m_index = index;
    }

    public void setStats(Vector3 home, float bounds, float maxAcc, float speed, float avoidance, float detection, float view)
    {
        m_homePos = home;
        m_homeBounds = bounds;
        m_maxAcceleration = maxAcc;
        m_speed = speed;
        m_avoidanceDistance = avoidance;
        m_detectionDistance = detection;
        m_viewAngle = view;
    }
    //Death behaviour
    private void OnDestroy()
    {
        m_parent.killBoid(m_index);
    }
}