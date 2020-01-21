using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_movementSpeed;


    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * m_movementSpeed * Time.deltaTime;
    }
}
