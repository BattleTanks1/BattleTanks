using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Selection : MonoBehaviour
{
    public GameObject m_selectionBox;
    public GameObject m_selectionBoxLeft;
    public GameObject m_selectionBoxBottom;
    public Rectangle m_AABB { get; private set; } = null;

    private void Start()
    {
        Assert.IsNotNull(m_selectionBox);
        m_selectionBox.SetActive(false);

        m_AABB = new Rectangle
            ((int)transform.position.x - (int)transform.localScale.x / 2,
            (int)transform.position.x + (int)transform.localScale.x / 2,
            (int)transform.position.z - (int)transform.localScale.z / 2,
            (int)transform.position.z + (int)transform.localScale.z / 2);
    }

    // Update is called once per frame
    private void Update()
    {
        m_selectionBox.transform.position = transform.position;
    }

    public void Select(Rectangle selectionBox)
    {
        if(selectionBox.contains(m_AABB))
        {
            m_selectionBox.SetActive(true);
        }
    }

    public void Deselect()
    {
        m_selectionBox.SetActive(false);
    }
}
