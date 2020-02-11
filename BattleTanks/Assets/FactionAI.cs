using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAIBehaviour
{
    Aggressive = 0,
    Passive
}

public class MessageToAIUnit
{
    public MessageToAIUnit(int targetID, int receiverID, eAIState messageType, Vector2Int lastTargetPosition)
    {
        m_targetID = targetID;
        m_receiverID = receiverID;
        m_messageType = messageType;
        m_lastTargetPosition = lastTargetPosition;
    }

    public int m_targetID { get; private set; }
    public int m_receiverID { get; private set; }
    public eAIState m_messageType { get; private set; }
    public Vector2Int m_lastTargetPosition { get; private set; }
}

public class FactionAI : Faction
{
    public FactionAI(eFactionName name) :
        base(name, eFactionControllerType.eAI)
    {
        m_receivedMessages = new Queue<MessageToAIController>();
        m_messagesToSend = new Queue<MessageToAIUnit>();
        m_visibleTargets = new HashSet<int>();
    }

    Queue<MessageToAIController> m_receivedMessages;
    Queue<MessageToAIUnit> m_messagesToSend;
    HashSet<int> m_visibleTargets;

    public override void update()
    {
        base.update();

        handleReceivedMessages();
        handleToSendMessages();

        foreach (Tank tank in m_tanks)
        {
            if (tank.m_currentState == eAIState.AwaitingDecision)
            {
                if (tank.m_scaredValue > 0 &&
                    InfluenceMap.Instance.isPositionInThreat(tank))
                {
                    tank.m_currentState = eAIState.SetDestinationToSafePosition;
                }
                else
                {
                    assignTankToAppropriateEnemy(tank);
                }
            }
            else if (tank.m_currentState == eAIState.TargetEnemy)
            {
                if (tank.m_scaredValue > 0 &&
                    InfluenceMap.Instance.isPositionInThreat(tank))
                {
                    tank.m_currentState = eAIState.SetDestinationToSafePosition;
                }
                else
                {
                    updateTankPositionToMoveTo(tank);
                }
            }
        }
    }

    public void addMessage(MessageToAIController newMessage)
    {
        m_receivedMessages.Enqueue(newMessage);
    }

    private void handleToSendMessages()
    {
        while (m_messagesToSend.Count > 0)
        {
            MessageToAIUnit messageToSend = m_messagesToSend.Dequeue();
            Tank tank = getTank(messageToSend.m_receiverID);
            switch (messageToSend.m_messageType)
            {
                case eAIState.TargetEnemy:
                    {
                        tank.m_targetID = messageToSend.m_targetID;
                        tank.m_currentState = messageToSend.m_messageType;
                        tank.m_positionToMoveTo = Utilities.convertToWorldPosition(messageToSend.m_lastTargetPosition);

                        Debug.Log("Shoot At Enemy");
                    }
                    break;
            }
        }
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
                        m_messagesToSend.Enqueue(new MessageToAIUnit(receivedMessage.m_targetID, receivedMessage.m_senderID,
                           eAIState.TargetEnemy, receivedMessage.m_lastTargetPosition));

                        m_visibleTargets.Add(receivedMessage.m_targetID);
                    }
                    break;
                case eAIUniMessageType.LostSightOfEnemy:
                    m_visibleTargets.Remove(receivedMessage.m_targetID);
                    break;
            }
        }
    }

    private Tank getTank(int ID)
    {
        Tank tank = null;
        foreach (Tank i in m_tanks)
        {
            if (i.m_ID == ID)
            {
                tank = i;
            }
        }

        return tank;
    }

    private bool isEnemyStillInSight(MessageToAIController receivedMessage)
    {
        Tank messageSender = getTank(receivedMessage.m_senderID);
        if (!messageSender)
        {
            return false;
        }

        Vector2Int senderPositionOnGrid = Utilities.convertToGridPosition(messageSender.transform.position);
        Rectangle searchableRect = new Rectangle(senderPositionOnGrid, messageSender.m_visibilityDistance);
        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                float distance = Vector2Int.Distance(senderPositionOnGrid, new Vector2Int(x, y));
                if (distance <= messageSender.m_visibilityDistance &&
                    fGameManager.Instance.getPointOnMap(y, x).tankID == receivedMessage.m_targetID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void assignTankToAppropriateEnemy(Tank tank)
    {
        if (m_visibleTargets.Count > 0)
        {
            int targetID = Utilities.INVALID_ID;
            float distance = float.MaxValue;
            Vector2Int tankPositionOnGrid = Utilities.convertToGridPosition(tank.transform.position);
            Vector2Int closestTargetPositionOnGrid = new Vector2Int();

            foreach (int visibleTargetID in m_visibleTargets)
            {
                Vector2Int targetPositionOnGrid = new Vector2Int();
                if (isTargetInSight(visibleTargetID, out targetPositionOnGrid))
                {
                    float i = Vector2Int.Distance(targetPositionOnGrid, tankPositionOnGrid);
                    if (i < distance)
                    {
                        distance = i;
                        targetID = visibleTargetID;
                        closestTargetPositionOnGrid = targetPositionOnGrid;
                    }
                }
            }

            tank.m_targetID = targetID;
            tank.m_currentState = eAIState.TargetEnemy;
            tank.m_positionToMoveTo = Utilities.convertToWorldPosition(closestTargetPositionOnGrid);
            Debug.Log(closestTargetPositionOnGrid.x);
            Debug.Log(closestTargetPositionOnGrid.y);
        }
    }

    private bool isTargetInSight(int targetID, out Vector2Int targetGridPosition)
    {
        Vector2Int position = new Vector2Int();
        bool targetFound = false;
        foreach (Tank tank in m_tanks)
        {
            Vector2Int positionOnGrid = Utilities.convertToGridPosition(tank.transform.position);
            Rectangle searchableRect = new Rectangle(positionOnGrid, tank.m_visibilityDistance);
            for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
            {
                for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
                {
                    float distance = Vector2Int.Distance(positionOnGrid, new Vector2Int(x, y));
                    if (distance <= tank.m_visibilityDistance &&
                        fGameManager.Instance.getPointOnMap(y, x).tankID == targetID)
                    {
                        position = new Vector2Int(x, y);
                        targetFound = true;
                        goto End;
                    }
                }
            }
        }

    End:
        targetGridPosition = position;
        return targetFound;
    }

    private void updateTankPositionToMoveTo(Tank tank)
    {
        Vector2Int targetOnGridPosition = new Vector2Int();
        if (isTargetInSight(tank.m_targetID, out targetOnGridPosition))
        {
            tank.m_positionToMoveTo = Utilities.convertToWorldPosition(targetOnGridPosition);
        }
        else
        {
            tank.m_targetID = Utilities.INVALID_ID;
            tank.m_currentState = eAIState.AwaitingDecision;
        }
    }

    private bool isSupported(Tank tank)
    { 
        return false;
    }

    private int closestTankNotInThreat()
    {
        float distance = float.MaxValue;



        return Utilities.INVALID_ID;
    }
}