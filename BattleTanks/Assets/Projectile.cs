using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float m_movementSpeed;
    [SerializeField]
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
            AITank tank = other.gameObject.GetComponent<AITank>();
            tank.damage(m_damage);
            Destroy(gameObject);
        }
    }
}
