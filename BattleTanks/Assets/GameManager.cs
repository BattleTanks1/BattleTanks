using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;


public class GameManager : MonoBehaviour
{
    public Faction[] m_factions;

    private int m_ID = 0; //Unique ID per ship

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

        m_factions = new Faction[(int)eFactionName.Total];
        m_factions[(int)eFactionName.Red] = new FactionHuman(eFactionName.Red);
        m_factions[(int)eFactionName.Blue] = new FactionAI(eFactionName.Blue);


    }

    private void Update()
    {
        foreach(Faction faction in m_factions)
        {
            faction.update();
        }
    }

    private Faction getPlayerFaction()
    {
        Faction playerFaction = null;
        foreach(Faction faction in m_factions)
        {
            if(faction.m_controllerType == eFactionControllerType.eHuman)
            {
                playerFaction = faction;
            }
        }

        return playerFaction;
    }

    public int addTank(Tank tank)
    {
        int ID = m_ID;
        ++m_ID;
        switch (tank.m_factionName)
        {
            case eFactionName.Red:
                m_factions[(int)tank.m_factionName].addTank(tank);
                break;
            case eFactionName.Blue:
                m_factions[(int)tank.m_factionName].addTank(tank);
                break;
        }

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

    public void selectPlayerUnits(fRectangle selectionBox)
    {
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        if(playerFaction == null)
        {
            return;
        }

        foreach (Tank tank in playerFaction.m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            if(tankSelection)
            {
                tankSelection.Select(selectionBox);
            }
        }
    }

    public void deselectPlayerUnits()
    {
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        if (playerFaction == null)
        {
            return;
        }

        foreach (Tank tank in playerFaction.m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            if (tankSelection)
            {
                tankSelection.Deselect();
            }
        }
    }

    public void moveSelectedPlayerUnitsToPosition(Vector3 position)
    {
        Faction playerFaction = getPlayerFaction();
        Assert.IsNotNull(playerFaction);
        if (playerFaction == null)
        {
            return;
        }

        foreach(Tank tank in playerFaction.m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            TankMovement tankMovement = tank.gameObject.GetComponent<TankMovement>();
            Assert.IsNotNull(tankMovement);

            if(tankSelection.isSelected())
            {
                tankMovement.moveTo(position);
            }
        }
    }
}