﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Selection : MonoBehaviour
{
    [SerializeField]
    private GameObject m_selectionBox;
    public fRectangle m_AABB { get; private set; } = null;

    private void Start()
    {
        Assert.IsNotNull(m_selectionBox);
        m_selectionBox.SetActive(false);

        m_AABB = new fRectangle(transform.position, transform.localScale);
    }

    // Update is called once per frame
    private void Update()
    {
        m_selectionBox.transform.position = 
            new Vector3( transform.position.x, m_selectionBox.transform.position.y, transform.position.z);

        m_AABB.reset(transform.position, transform.localScale);
    }

    public bool isSelected()
    {
        return m_selectionBox.activeSelf;
    }

    public void Select(fRectangle selectionBox)
    {
        if(selectionBox.contains(m_AABB))
        {
            m_selectionBox.SetActive(true);
        }
        else
        {
            m_selectionBox.SetActive(false);
        }
    }

    public void Deselect()
    {
        m_selectionBox.SetActive(false);
    }
}