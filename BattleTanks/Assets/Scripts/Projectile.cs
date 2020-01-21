using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_movementSpeed;
    private int m_damage;

    private void Start()
    {
        m_damage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * m_movementSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Tank")
        {
            Tank tank = other.gameObject.GetComponent<Tank>();
            tank.damage(m_damage);
            Destroy(gameObject);
        }
    }
}
