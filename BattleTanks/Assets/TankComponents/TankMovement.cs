using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public Rigidbody m_rigidbody;

    float m_movementSpeed;
    public float m_maxSpeed = 20;
    
    float m_rotationSpeed;
    public float m_maxRotation = 50;

    public void forward()
    {
        m_movementSpeed += m_maxSpeed;
    }

    public void backward()
    {
        m_movementSpeed -= m_maxSpeed;
    }

    public void leftTurn()
    {
        m_rotationSpeed += m_maxRotation;
    }

    public void rightTurn()
    {
        m_rotationSpeed += -m_maxRotation;
    }

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotation
        float turn = m_rotationSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_rigidbody.MoveRotation(m_rigidbody.rotation * turnRotation);

        //Movement
        Vector3 movement = transform.forward * m_movementSpeed * Time.deltaTime;
        m_rigidbody.MovePosition(m_rigidbody.position + movement);

        m_movementSpeed = 0;
        m_rotationSpeed = 0;
    }
}
