using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        fGameManager.Instance.m_player = this;
        m_faction = Faction.player;

    }

    // Update is called once per frame
    protected override void Update()
    {
        //Rotation
        if(Input.GetKey(KeyCode.D))
        {
            rightTurn(Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A))
        {
            leftTurn(Time.deltaTime);
        }
        //Movement
        if(Input.GetKey(KeyCode.W))
        {
            forward(Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            backward(Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            shoot();
        }

        base.Update();
    }
}