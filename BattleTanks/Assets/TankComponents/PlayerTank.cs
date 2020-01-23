using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TankCore>().m_faction = Faction.player;
    }

    // Update is called once per frame
    void Update()
    {
        TankMovement move = GetComponent<TankMovement>();
        //Rotation
        if (Input.GetKey(KeyCode.D))
        {
            move.rightTurn(Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A))
        {
            move.leftTurn(Time.deltaTime);
        }
        //Movement
        if(Input.GetKey(KeyCode.W))
        {
            move.forward(Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            move.backward(Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Shoot
        }

        //Find the mouse position relative to the tank position
            //Get mouse position on screen frame
            //Get tank position on screen frame
            //Get vector between them
            //Set as new target angle for turret
    }
}