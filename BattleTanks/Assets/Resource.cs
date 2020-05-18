using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField]
    private int m_resourceAmount = 1000;

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.addResource(this);   
    }

    public int extractResource()
    {
        --m_resourceAmount;
        return 1;
    }
}