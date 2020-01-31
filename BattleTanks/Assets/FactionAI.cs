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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        

    }

    // Update is called once per frame
    protected override void Update()
    {
        switch (m_currentState)
        {
            case eAIControllerState.MakeInitialDecision:
                
                break;
        }
    }
}
