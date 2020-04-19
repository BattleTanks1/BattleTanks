using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FactionPlayer : Faction
{
    private static FactionPlayer _instance;
    public static FactionPlayer Instance { get { return _instance; } }

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
    }

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void moveSelectedUnitsToPosition(Vector3 position)
    {
        foreach (Tank tank in m_tanks)
        {
            Selection tankSelection = tank.gameObject.GetComponent<Selection>();
            Assert.IsNotNull(tankSelection);
            if(!tankSelection.isSelected())
            {
                continue;
            }

            TankMovement tankMovement = tank.gameObject.GetComponent<TankMovement>();
            Assert.IsNotNull(tankMovement);

            tankMovement.moveTo(position);
        }
    }
}