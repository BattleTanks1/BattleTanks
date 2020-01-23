﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField]
    protected float m_movementSpeed;

    [SerializeField]
    protected float m_maxSpeed = 20;

    [SerializeField]
    protected float m_rotationSpeed;

    [SerializeField]
    protected float m_maxRotation = 50;

    protected void forward(float dTime)
    {
        m_movementSpeed += m_maxSpeed;
    }

    protected void backward(float dTime)
    {
        m_movementSpeed += -m_maxSpeed;
    }

    protected void leftTurn(float dTime)
    {
        m_rotationSpeed += m_maxRotation;
    }

    protected void rightTurn(float dTime)
    {
        m_rotationSpeed += -m_maxRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, m_rotationSpeed * Time.deltaTime, 0));
        transform.Translate(transform.forward * m_movementSpeed * dTime);
        m_movementSpeed = 0;
        m_rotationSpeed = 0;
    }
}
