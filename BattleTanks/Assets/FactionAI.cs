using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions;

public enum eAIUniMessageType
{
    EnemySpottedAtPosition = 0,
    LostSightOfEnemy
}

public class MessageToAIController
{
    public MessageToAIController(int targetID, Vector2Int lastTargetPosition, eAIUniMessageType messageType, int senderID, eFactionName senderFaction)
    {
        m_targetID = targetID;
        m_lastTargetPosition = lastTargetPosition;
        m_messageType = messageType;
        m_senderID = senderID;
        m_senderFaction = senderFaction;
    }

    public MessageToAIController(int targetID, eAIUniMessageType messageType, eFactionName senderFaction)
    {
        m_targetID = targetID;
        m_messageType = messageType;
    }

    public Vector2Int m_lastTargetPosition { get; private set; }
    public int m_targetID { get; private set; }
    public eAIUniMessageType m_messageType { get; private set; }
    public int m_senderID { get; private set; }
    public eFactionName m_senderFaction { get; private set; }
}

public class FactionAI : Faction
{
    private Queue<MessageToAIController> m_receivedMessages;
    private HashSet<int> m_visibleTargets;

    protected override void Awake()
    {
        base.Awake();
        m_controllerType = eFactionControllerType.AI;
        
        m_receivedMessages = new Queue<MessageToAIController>();
        m_visibleTargets = new HashSet<int>();
    }

    protected override void Start()
    {
        base.Start();
        m_building.setWayPoint(new Vector3(110, 1, 70));
        addUnit(m_building.spawnUnit(eUnitType.Attacker));   
    }

    protected override void Update() 
    {
        base.Update();

        handleReceivedMessages();

        //foreach (Tank tank in m_tanks)
        //{
        //    TankStateHandler stateHandlerComponent = tank.gameObject.GetComponent<TankStateHandler>();
        //    Assert.IsNotNull(stateHandlerComponent);

        //    if (stateHandlerComponent.m_currentState == eAIState.AwaitingDecission)
        //    {
        //        assignTankToEnemyInRange(tank);
        //    }
        //}
    }

    public void addMessage(MessageToAIController newMessage)
    {
        m_receivedMessages.Enqueue(newMessage);
    }

    private void handleReceivedMessages()
    {
        while (m_receivedMessages.Count > 0)
        {
            MessageToAIController receivedMessage = m_receivedMessages.Dequeue();
            switch (receivedMessage.m_messageType)
            {
                case eAIUniMessageType.EnemySpottedAtPosition:
                    if (isEnemyStillInSight(receivedMessage))
                    {
                        UnitStateHandler stateHandlerComponent = getTank(receivedMessage.m_senderID).gameObject.GetComponent<UnitStateHandler>();
                        Assert.IsNotNull(stateHandlerComponent);

                        stateHandlerComponent.switchToState(eUnitState.AttackingEnemy, receivedMessage.m_targetID, 
                            Utilities.convertToWorldPosition(receivedMessage.m_lastTargetPosition));
                    }
                    break;
                case eAIUniMessageType.LostSightOfEnemy:
                    m_visibleTargets.Remove(receivedMessage.m_targetID);
                    break;
            }
        }
    }

    private Unit getTank(int ID)
    {
        Unit unit = null;
        foreach (Unit i in m_units)
        {
            if (i.getID() == ID)
            {
                unit = i;
            }
        }

        return unit;
    }

    private bool isEnemyStillInSight(MessageToAIController receivedMessage)
    {
        Unit messageSender = getTank(receivedMessage.m_senderID);
        if (!messageSender)
        {
            return false;
        }

        Vector2Int senderPositionOnGrid = Utilities.convertToGridPosition(messageSender.transform.position);
        iRectangle searchableRect = new iRectangle(senderPositionOnGrid, messageSender.getVisibilityDistance());
        for (int y = searchableRect.m_bottom; y <= searchableRect.m_top; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                Vector2Int vBetween = senderPositionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if(pointOnMap == null)
                {
                    continue;
                }

                if (pointOnMap.unitID == receivedMessage.m_targetID &&
                    vBetween.sqrMagnitude <= messageSender.getVisibilityDistance() * messageSender.getVisibilityDistance())
                {
                    return true;
                }
            }
        }

        return false;
    }

    //private void assignTankToEnemyInRange(Unit unit)
    //{
    //    iRectangle searchRect = new iRectangle(Utilities.convertToGridPosition(unit.transform.position), unit.getVisibilityDistance());
    //    for (int y = searchRect.m_bottom; y <= searchRect.m_top; ++y)
    //    {
    //        for (int x = searchRect.m_left; x <= searchRect.m_right; ++x)
    //        {
    //            Vector2Int positionOnGrid = new Vector2Int(x, y);
    //            int targetID = Utilities.INVALID_ID;
    //            Vector2Int vBetween = Utilities.convertToGridPosition(unit.transform.position) - positionOnGrid;
    //            if (vBetween.sqrMagnitude <= unit.getVisibilityDistance() * unit.getVisibilityDistance() &&
    //                Map.Instance.isEnemyOnPosition(positionOnGrid, unit.getFactionName(), out targetID))
    //            {
    //                Debug.Log("Enemy Spotted");
    //                Assert.IsTrue(targetID != Utilities.INVALID_ID);

    //                UnitStateHandler stateHandlerComponent = unit.gameObject.GetComponent<UnitStateHandler>();
    //                Assert.IsNotNull(stateHandlerComponent);
    //                stateHandlerComponent.switchToState(eUnitState.MovingToNewPosition, targetID, Utilities.convertToWorldPosition(positionOnGrid));
    //            }
    //        }
    //    }
    //}
}