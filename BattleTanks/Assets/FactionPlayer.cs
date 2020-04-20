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

    // Update is called once per frame
    void Update()
    {
        
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
        Selection buildingSelection = m_building.GetComponent<Selection>();
        Assert.IsNotNull(buildingSelection);
        if (buildingSelection.isSelected())
        {
            m_building.setWayPoint(position);
        }
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

                TankPlayer tankPlayer = tank.GetComponent<TankPlayer>();
                Assert.IsNotNull(tankPlayer);

                tankPlayer.receiveMessage(new MessageToAIUnit(Utilities.INVALID_ID, eAIState.MovingToNewPosition,
                    Utilities.convertToGridPosition(position)));
            }
        }
    }

    //public void assignBuildingWayPoint(Vector3 position)
    //{
    //    Selection buildingSelection = m_building.GetComponent<Selection>();
    //    Assert.IsNotNull(buildingSelection);
    //    Debug.Log("Hit");
    //    Assert.IsTrue(buildingSelection.isSelected());
    //    if(buildingSelection.isSelected())
    //    {
    //        m_building.setWayPoint(position);
    //    }
    //}

    //public void moveSelectedUnitsToPosition(Vector3 position)
    //{
       
    //}

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
                TankPlayer tankPlayer = tank.gameObject.GetComponent<TankPlayer>();
                Assert.IsNotNull(tankPlayer);

                tankPlayer.receiveMessage(new MessageToAIUnit(enemy.m_ID, eAIState.MovingToNewPosition,
                    Utilities.convertToGridPosition(enemy.transform.position)));
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
}