using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Building : MonoBehaviour
{
    [SerializeField]
    private Tank m_spawnableUnit = null;
    [SerializeField]
    private GameObject m_wayPointPrefab = null;
    private GameObject m_wayPointClone = null;
    
    private void Awake()
    {
        Assert.IsNotNull(m_spawnableUnit);
        Assert.IsNotNull(m_wayPointPrefab);

        m_wayPointClone = Instantiate(m_wayPointPrefab, transform.position, Quaternion.identity);
    }

    public void setWayPoint(Vector3 position)
    {
        m_wayPointClone.SetActive(true);
        m_wayPointClone.transform.position = new Vector3(position.x, 1, position.z);
    }

    public void showWayPoint()
    {
        m_wayPointClone.SetActive(true);
    }

    public void hideWayPoint()
    {
        m_wayPointClone.SetActive(false);
    }

    public Tank spawnUnit()
    {
        Selection selectionComponent = GetComponent<Selection>();
        Assert.IsNotNull(selectionComponent);

        Vector3 startingPosition = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f));
        int distance = 1;
        Tank newTank = null;

        while(!newTank)
        {
            Vector3 spawnPosition = transform.position + startingPosition.normalized * distance;
            spawnPosition = new Vector3(spawnPosition.x, 1, spawnPosition.z);
            if(!selectionComponent.contains(spawnPosition))
            {
                newTank = Instantiate(m_spawnableUnit, spawnPosition, Quaternion.identity);
            }

            ++distance;
        }

        return newTank;
    }
}