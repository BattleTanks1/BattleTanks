using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField]
    private int m_resourceCount = 10;

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.addResource(this);   
    }


    public int extractResource(int extractAmount)
    {
        --m_resourceCount;

        return extractAmount;
    }
}