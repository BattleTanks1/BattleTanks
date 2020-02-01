using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAIBehaviour
{
    Aggressive = 0,
    Passive
}

public enum eAIControllerState
{
    MakeInitialDecision = 0
}

public class FactionAI : Faction
{
    public FactionAI(eFactionName name) : 
        base(name, eFactionControllerType.eAI)
    {
       
    }

    eAIBehaviour m_behaviour;
    eAIControllerState m_currentState;


    // Update is called once per frame
    //protected override void Update()
    //{
    //    base.Start();

    //    switch (m_currentState)
    //    {
    //        case eAIControllerState.MakeInitialDecision:
    //            foreach(Tank tank in m_tanks)
    //            {
    //                AITank tankAI = tank as AITank;
                    
    //            }
    //            break;
    //    }
    //}
}