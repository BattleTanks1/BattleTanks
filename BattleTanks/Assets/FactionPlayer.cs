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
        //Handle units
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

                UnitStateHandler unitStateHandler = unit.GetComponent<UnitStateHandler>();
                Assert.IsNotNull(unitStateHandler);

                if(m_attackMoveNextSelection)
                {
                    unitStateHandler.switchToState(eTankState.SetAttackDestination, Utilities.INVALID_ID, position);
                }
                else
                {
                    unitStateHandler.switchToState(eTankState.SetNewDestination, Utilities.INVALID_ID, position);
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
                UnitStateHandler unitStateHandler = tank.gameObject.GetComponent<UnitStateHandler>();
                Assert.IsNotNull(unitStateHandler);

                unitStateHandler.switchToState(eTankState.MovingToNewPosition, enemy.m_ID, enemy.transform.position);
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