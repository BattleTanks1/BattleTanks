using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_faction = Faction.player;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //Rotation
        if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
        }
        else if(Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
        }
        //Movement
        else if(Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * m_movementSpeed * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * m_movementSpeed * Time.deltaTime;
        }

        else if(Input.GetKeyDown(KeyCode.Space))
        {
            shoot();
        }
    }
}