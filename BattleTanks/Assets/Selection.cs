using System.Collections;
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

        m_AABB = new fRectangle
            (transform.position.x - transform.localScale.x / 2.0f,
            transform.position.x + transform.localScale.x / 2.0f,
            transform.position.z - transform.localScale.z / 2.0f,
            transform.position.z + transform.localScale.z / 2.0f);
    }

    // Update is called once per frame
    private void Update()
    {
        m_selectionBox.transform.position = m_selectionBox.transform.position;
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