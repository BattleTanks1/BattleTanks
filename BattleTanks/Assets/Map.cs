using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    int m_width;
    [SerializeField]
    int m_height;

    int[,] m_map;

    private void Awake()
    {
        m_map = new int[m_height, m_width];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}