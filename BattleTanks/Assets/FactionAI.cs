using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FactionAI : Faction
{
    private Queue<MessageToAIController> m_receivedMessages;
    private HashSet<int> m_visibleTargets;

    private void Awake()
    {
        m_receivedMessages = new Queue<MessageToAIController>();
        m_visibleTargets = new HashSet<int>();
    }

    private void Start()
    {
        m_controllerType = eFactionControllerType.AI;
    }

    private void Update() 
    {
        handleReceivedMessages();

        foreach (Tank tank in m_tanks)
        {
            AITank aiComponent = tank.gameObject.GetComponent<AITank>();
            Assert.IsNotNull(aiComponent);

            if (aiComponent.m_currentState == eAIState.AwaitingDecision)
            {
                assignTankToEnemyInRange(tank);
            }
        }
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
                        AITank aiComponent = getTank(receivedMessage.m_senderID).gameObject.GetComponent<AITank>();
                        Assert.IsNotNull(aiComponent);
                        aiComponent.switchToState(eAIState.ShootingAtEnemy, receivedMessage.m_targetID, Utilities.convertToWorldPosition(receivedMessage.m_lastTargetPosition));
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
                Vector2Int vBetween = senderPositionOnGrid - new Vector2Int(x, y);
                PointOnMap pointOnMap = Map.Instance.getPoint(x, y);
                if(pointOnMap == null)
                {
                    continue;
                }

                if (pointOnMap.tankID == receivedMessage.m_targetID &&
                    vBetween.sqrMagnitude <= messageSender.m_visibilityDistance * messageSender.m_visibilityDistance)
                {
                    return true;
                }
            }
        }

        return false;
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
                Vector2Int vBetween = Utilities.convertToGridPosition(tank.transform.position) - positionOnGrid;
                if (vBetween.sqrMagnitude <= tank.m_visibilityDistance * tank.m_visibilityDistance &&
                    Map.Instance.isEnemyOnPosition(positionOnGrid, tank.m_factionName, out targetID))
                {
                    Debug.Log("Enemy Spotted");
                    Assert.IsTrue(targetID != Utilities.INVALID_ID);

                    AITank aiComponent = tank.gameObject.GetComponent<AITank>();
                    Assert.IsNotNull(aiComponent);
                    aiComponent.switchToState(eAIState.MovingToNewPosition, targetID, Utilities.convertToWorldPosition(positionOnGrid));
                }
            }
        }
    }
}