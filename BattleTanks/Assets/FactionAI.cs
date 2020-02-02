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
    MakeInitialDecision = 0,
    Maintain
}

public class MessageToAIUnit
{
    public MessageToAIUnit(int targetID, int receiverID, eAIState messageType)
    {
        m_targetID = targetID;
        m_receiverID = receiverID;
        m_messageType = messageType;
    }

    public int m_targetID { get; private set; }
    public int m_receiverID { get; private set; }
    public eAIState m_messageType { get; private set; }
}

public class FactionAI : Faction
{
    public FactionAI(eFactionName name) : 
        base(name, eFactionControllerType.eAI)
    {
        m_receivedMessages = new Queue<MessageToAIController>();
        m_messagesToSend = new Queue<MessageToAIUnit>();
    }

    eAIBehaviour m_behaviour;
    eAIControllerState m_currentState;
    Queue<MessageToAIController> m_receivedMessages;
    Queue<MessageToAIUnit> m_messagesToSend;

    public override void update()
    {
        base.update();

        handleReceivedMessages();
        handleToSendMessages();
    }

    public void addMessage(MessageToAIController newMessage)
    {
        m_receivedMessages.Enqueue(newMessage);
    }

    private void handleToSendMessages()
    {
        while(m_messagesToSend.Count > 0)
        {
            MessageToAIUnit messageToSend = m_messagesToSend.Dequeue();
            Tank tank = getTank(messageToSend.m_receiverID);
            switch (messageToSend.m_messageType)
            {
                case eAIState.ShootAtEnemy:
                    {
                        tank.m_targetID = messageToSend.m_targetID;
                        tank.m_currentState = messageToSend.m_messageType;
                    }
                    break;
            }
        }
    }

    private void handleReceivedMessages()
    {
        while(m_receivedMessages.Count > 0)
        {
            MessageToAIController receivedMessage = m_receivedMessages.Dequeue();
            switch (receivedMessage.m_messageType)
            {
                case eAIUniMessageType.EnemySpottedAtPosition:
                    if(isEnemyStillInSight(receivedMessage))
                    {
                        m_messagesToSend.Enqueue(new MessageToAIUnit(receivedMessage.m_targetID, receivedMessage.m_senderID, 
                           eAIState.ShootAtEnemy));
                    }
                    break;
            }
        }
    }

    private Tank getTank(int ID)
    {
        Tank tank = null;
        foreach(Tank i in m_tanks)
        {
            if(i.m_ID == ID)
            {
                tank = i;
            }
        }

        return tank;
    }

    private bool isEnemyStillInSight(MessageToAIController receivedMessage)
    {
        Tank messageSender = getTank(receivedMessage.m_senderID);
        if(!messageSender)
        {
            return false;
        }

        Vector2Int senderPositionOnGrid = Utilities.convertToGridPosition(messageSender.transform.position);
        SearchRect searchableRect = new SearchRect(senderPositionOnGrid, messageSender.m_visibilityDistance);
        for(int y = searchableRect.top; y <= searchableRect.bottom; ++y)
        {
            for(int x = searchableRect.left; x <= searchableRect.right; ++x)
            {
                float distance = Vector2Int.Distance(senderPositionOnGrid, new Vector2Int(x, y));
                if(distance <= messageSender.m_visibilityDistance &&
                    fGameManager.Instance.getPointOnMap(y, x).tankID == receivedMessage.m_targetID)
                {
                    return true;
                }
            }
        }

        return false;
    }
}