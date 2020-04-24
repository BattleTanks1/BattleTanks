using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FactionPlayer : Faction
{
    [SerializeField]
    private Building m_building = null;
    private bool m_attackMoveNextSelection = false;

    private void Awake()
    {
        Assert.IsNotNull(m_building);
        m_controllerType = eFactionControllerType.Human;
    }

    public void selectUnits(fRectangle selectionBox)
    {
        foreach (Unit unit in m_unit)
        {
            Selection unitSelection = unit.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(unitSelection);

            unitSelection.Select(selectionBox);
        }
    }

    public void deselectAllUnits()
    { 
        foreach (Unit unit in m_unit)
        {
            Selection unitSelection = unit.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(unitSelection);

            unitSelection.Deselect();
        }

        Selection buildingSelection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(buildingSelection);
        buildingSelection.Deselect();
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
            foreach (Unit unit in m_unit)
            {
                Selection unitSelection = unit.gameObject.GetComponent<Selection>();
                Assert.IsNotNull(unitSelection);
                if (!unitSelection.isSelected())
                {
                    continue;
                }

                TankStateHandler tankStateHandler = unit.GetComponent<TankStateHandler>();
                Assert.IsNotNull(tankStateHandler);

                if(m_attackMoveNextSelection)
                {
                    tankStateHandler.switchToState(eTankState.SetAttackDestination, Utilities.INVALID_ID, position);
                }
                else
                {
                    tankStateHandler.switchToState(eTankState.SetNewDestination, Utilities.INVALID_ID, position);
                }
            }
        }
    }

    public void targetEnemyAtPosition(Vector3 position)
    {
        Unit enemy = GameManager.Instance.getUnit(position);
        if(!enemy)
        {
            return;
        }
        
        foreach (Unit tank in m_unit)
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
        Selection buildingSelection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(buildingSelection);

        m_building.showWayPoint();
        buildingSelection.select(position);
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
                Unit unit = newGameObject.GetComponent<Unit>();
                m_unit.Add(unit);
            }
        }
    }

    public void turnOnAttackMove()
    {
        m_attackMoveNextSelection = true;
    }

    public void turnOffAttackMove()
    {
        m_attackMoveNextSelection = false;
    }
}