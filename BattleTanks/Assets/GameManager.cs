using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public Faction[] m_factions;

    private int m_ID = 0; //Unique ID

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    public Faction getPlayerFaction()
    {
        Faction playerFaction = null;
        foreach(Faction faction in m_factions)
        {
            if(faction.getControllerType() == eFactionControllerType.Human)
            {
                playerFaction = faction;
            }
        }

        return playerFaction;
    }

    public int addUnit(Tank tank)
    {
        int ID = m_ID;
        ++m_ID;

        return ID;
    }

    public void sendAIControllerMessage(MessageToAIController message)
    {
        switch (message.m_senderFaction)
        {
            case eFactionName.Red:
                {
                    FactionAI faction = m_factions[(int)message.m_senderFaction] as FactionAI;
                    faction.addMessage(message);
                }
                break;
            case eFactionName.Blue:
                {
                    FactionAI faction = m_factions[(int)message.m_senderFaction] as FactionAI;
                    faction.addMessage(message);
                }
                break;
        }
    }

    public Vector3 getTankPosition(int tankID)
    {
        Assert.IsTrue(tankID != Utilities.INVALID_ID);

        foreach (Faction faction in m_factions)
        {
            foreach(Tank tank in faction.m_tanks)
            {
                if(tank.m_ID == tankID)
                {
                    return tank.transform.position;
                }
            }
        }

        return Utilities.INVALID_POSITION;
    }

    public void targetEnemyAtPosition(Vector3 position)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        
        foreach(Tank tank in playerFaction.m_tanks)
        {
            Selection selectionComponent = tank.GetComponent<Selection>();
            Assert.IsNotNull(selectionComponent);

            if(selectionComponent.isSelected())
            {
                
            }
        }
    }
}