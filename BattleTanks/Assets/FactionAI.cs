using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions;

public class FactionAI : Faction
{
    [SerializeField]
    private float m_timeBetweenAttackerSpawn = 3.0f;

    protected override void Awake()
    {
        base.Awake();
        m_controllerType = eFactionControllerType.AI;
    }

    protected override void Start()
    {
        base.Start();
        m_building.setWayPoint(new Vector3(175, 1, 90));
        StartCoroutine(spawnAttacker());
    }

    protected override void Update() 
    {
        base.Update();
    }

    private IEnumerator spawnAttacker()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(m_timeBetweenAttackerSpawn);

            addUnit(m_building.spawnUnit(eUnitType.Attacker));
        }
    }
}