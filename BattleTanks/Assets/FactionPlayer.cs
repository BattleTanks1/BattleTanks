using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FactionPlayer : Faction
{
    [SerializeField]
    private Building m_building = null;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(m_building);
        m_controllerType = eFactionControllerType.Human;
    }

    public void selectUnits(fRectangle selectionBox)
    {
        foreach (Tank tank in m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);

            tankSelection.Select(selectionBox);
        }
    }

    public void deselectAllUnits()
    { 
        foreach (Tank tank in m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);

            tankSelection.Deselect();
        }

        Selection selection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(selection);
        selection.Deselect();
        m_building.hideWayPoint();
    }

    public void handleSelectedUnit(Vector3 position)
    {
        //Handle selected building
        Selection buildingSelection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(buildingSelection);
        if (buildingSelection.isSelected())
        {
            m_building.setWayPoint(position);
        }
        //Handle tanks
        else
        {
            foreach (Tank tank in m_tanks)
            {
                Selection tankSelection = tank.gameObject.GetComponent<Selection>();
                Assert.IsNotNull(tankSelection);
                if (!tankSelection.isSelected())
                {
                    continue;
                }

                TankStateHandler tankStateHandler = tank.GetComponent<TankStateHandler>();
                Assert.IsNotNull(tankStateHandler);

                tankStateHandler.switchToState(eTankState.MovingToNewPosition, Utilities.INVALID_ID, position);
            }
        }
    }

    public void targetEnemyAtPosition(Vector3 position)
    {
        Tank enemy = GameManager.Instance.getTank(position);
        if(!enemy)
        {
            return;
        }
        
        foreach (Tank tank in m_tanks)
        {
            Selection selectionComponent = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(selectionComponent);

            if (selectionComponent.isSelected())
            {
                TankStateHandler tankStateHandler = tank.gameObject.GetComponent<TankStateHandler>();
                Assert.IsNotNull(tankStateHandler);

                tankStateHandler.switchToState(eTankState.MovingToNewPosition, enemy.m_ID, enemy.transform.position);
            }
        }
    }

    public void selectBuilding(Vector3 position)
    {
        Selection selection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(selection);

        m_building.showWayPoint();
        selection.select(position);
    }

    public void spawnUnit()
    {
        Selection buildingSelection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(buildingSelection);

        if(buildingSelection.isSelected())
        {
            GameObject newGameObject = m_building.spawnUnit();
            if(newGameObject)
            {
                Tank tankComponent = newGameObject.GetComponent<Tank>();
                m_tanks.Add(tankComponent);
            }
        }
    }
}