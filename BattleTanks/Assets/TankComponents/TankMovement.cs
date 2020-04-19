using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TankMovement : MonoBehaviour
{
    public Vector3 m_oldPosition { get; private set; }

    [SerializeField]
    private float m_movementSpeed = 0.0f;
    private Vector3 m_positionToMoveTo;
    private Tank m_tank = null;
    private bool m_reachedDestination = true;

    // Start is called before the first frame update
    private void Start()
    {
        m_tank = GetComponent<Tank>();
        Assert.IsNotNull(m_tank);
        m_oldPosition = transform.position;

        Map.Instance.updatePositionOnMap(m_oldPosition, transform.position, m_tank.m_factionName, m_tank.m_ID);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!m_reachedDestination)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, m_positionToMoveTo, m_movementSpeed * Time.deltaTime);
            if(!Map.Instance.isPositionOccupied(newPosition, m_tank.m_ID))
            {
                m_oldPosition = transform.position;
                transform.position = newPosition;
                Map.Instance.updatePositionOnMap(m_oldPosition, transform.position, m_tank.m_factionName, m_tank.m_ID);

                if (transform.position == m_positionToMoveTo)
                {
                    m_reachedDestination = true;
                }
            }
        }
    } 

    private void Move()
    {
        float step = m_movementSpeed * Time.deltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, m_positionToMoveTo, step);
        if (!Map.Instance.isPositionOccupied(newPosition, m_tank.m_ID))
        {
            m_oldPosition = transform.position;
            transform.position = newPosition;
            Map.Instance.updatePositionOnMap(m_oldPosition, transform.position, m_tank.m_factionName, m_tank.m_ID);
        }
    }

    public void moveTo(Vector3 position)
    {
        m_positionToMoveTo = position;
        m_reachedDestination = false;
    }

    public bool reachedDestination()
    {
        return m_positionToMoveTo == transform.position;
    }

    public void stop()
    {
        m_reachedDestination = true;
    }
}