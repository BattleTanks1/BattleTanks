using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Building : MonoBehaviour
{
    [SerializeField]
    private GameObject m_spawnableUnit = null;
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
}