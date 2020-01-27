using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    public Vector3 m_newPosition;
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
        base.Update();
    }
}