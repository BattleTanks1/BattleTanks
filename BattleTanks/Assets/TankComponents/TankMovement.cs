using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TankMovement : MonoBehaviour
{
    public Vector3 m_oldPosition { get; private set; }
    public Vector3 m_velocity;
    public Vector3 m_positionToMoveTo;
    public float m_movementSpeed;

    private Tank m_tank = null;

    // Start is called before the first frame update
    private void Start()
    {
        m_tank = GetComponent<Tank>();
        Assert.IsNotNull(m_tank);
        m_oldPosition = transform.position;

        GameManager.Instance.updatePositionOnMap(m_tank, this);
    }

    // Update is called once per frame
    private void Update()
    {
        
    } 

    private void Move()
    {
        float step = m_movementSpeed * Time.deltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, m_positionToMoveTo, step);
        if (!GameManager.Instance.isPositionOccupied(newPosition, m_tank.m_ID))
        {
            m_oldPosition = transform.position;
            transform.position = newPosition;
            GameManager.Instance.updatePositionOnMap(m_tank, this);
        }
    }

    public void moveTo(Vector3 newPosition)
    {

    }

    public bool reachedDestination()
    {
        return m_positionToMoveTo == transform.position;
    }
}
