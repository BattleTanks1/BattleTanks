using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum eAIBehaviour
{
    Aggressive = 0,
    Passive
}

public class MessageToAIUnit
{
    public MessageToAIUnit(int targetID, eAIState messageType, Vector2Int position)
    {
        m_targetID = targetID;
        m_receiverID = Utilities.INVALID_ID;
        m_messageType = messageType;
        m_lastTargetPosition = position;
    }

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
            AITank aiComponent = tank.gameObject.GetComponent<AITank>();
            Assert.IsNotNull(aiComponent);

            if(aiComponent.m_currentState == eAIState.AwaitingDecision)
            {
                assignTankToEnemyInRange(tank);
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
            AITank aiComponent = getTank(messageToSend.m_receiverID).gameObject.GetComponent<AITank>();
            Assert.IsNotNull(aiComponent);
            if(aiComponent)
            {
                aiComponent.receiveMessage(messageToSend);
            }
            switch (messageToSend.m_messageType)
            {
                case eAIState.ShootingAtEnemy:
                {

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
                           eAIState.ShootingAtEnemy, receivedMessage.m_lastTargetPosition));

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
        iRectangle searchableRect = new iRectangle(senderPositionOnGrid, messageSender.m_visibilityDistance);
        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                float distance = Vector2Int.Distance(senderPositionOnGrid, new Vector2Int(x, y));
                if (distance <= messageSender.m_visibilityDistance &&
                    GameManager.Instance.getPointOnMap(y, x).tankID == receivedMessage.m_targetID)
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
                if (isTargetInSightAllTanks(visibleTargetID, out targetPositionOnGrid))
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

            AITank aiComponent = tank.gameObject.GetComponent<AITank>();
            Assert.IsNotNull(aiComponent);
            if(aiComponent)
            {
                aiComponent.receiveMessage(new MessageToAIUnit(targetID, eAIState.ShootingAtEnemy, closestTargetPositionOnGrid));
            }
        }
    }

    private bool isTargetInSightAllTanks(int targetID, out Vector2Int targetGridPosition)
    {
        Vector2Int position = new Vector2Int();
        bool targetFound = false;
        foreach (Tank tank in m_tanks)
        {
            Vector2Int positionOnGrid = Utilities.convertToGridPosition(tank.transform.position);
            iRectangle searchableRect = new iRectangle(positionOnGrid, tank.m_visibilityDistance);
            for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
            {
                for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
                {
                    float distance = Vector2Int.Distance(positionOnGrid, new Vector2Int(x, y));
                    if (distance <= tank.m_visibilityDistance &&
                        GameManager.Instance.getPointOnMap(y, x).tankID == targetID)
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

    private void assignTankToEnemyInRange(Tank tank)
    {
        iRectangle searchRect = new iRectangle(Utilities.convertToGridPosition(tank.transform.position), tank.m_visibilityDistance);
        for (int y = searchRect.m_top; y <= searchRect.m_bottom; ++y)
        {
            for (int x = searchRect.m_left; x <= searchRect.m_right; ++x)
            {
                Vector2Int positionOnGrid = new Vector2Int(x, y);
                int targetID = Utilities.INVALID_ID;
                if (Vector2Int.Distance(Utilities.convertToGridPosition(tank.transform.position), positionOnGrid) <= tank.m_visibilityDistance &&
                    GameManager.Instance.isEnemyOnPosition(positionOnGrid, tank.m_factionName, out targetID))
                {
                    Debug.Log("Enemy Spotted");
                    Assert.IsTrue(targetID != Utilities.INVALID_ID);
                    AITank aiComponent = tank.gameObject.GetComponent<AITank>();
                    Assert.IsNotNull(aiComponent);

                    MessageToAIUnit message = new MessageToAIUnit(targetID, eAIState.MovingToNewPosition, positionOnGrid);
                    aiComponent.receiveMessage(message);
                }
            }
        }
    }
}