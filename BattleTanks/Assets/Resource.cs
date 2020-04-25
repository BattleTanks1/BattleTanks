using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField]
    private int m_resourceCount = 10;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.addResource(this);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}