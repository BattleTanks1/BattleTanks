﻿using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    public GameObject m_selectionBoxPrefab = null;
    private GameObject m_selectionBoxClone = null;

    [SerializeField]
    private float m_zoomSpeed = 0.0f;
    [SerializeField]
    private float m_maximumMagnification = 0.0f;
    [SerializeField]
    private float m_selectionBoxHeight = 0.5f;
    [SerializeField]
    private float m_speed = 30.0f;
    [SerializeField]
    private float m_borderOffset = 20.0f;
    [SerializeField]
    private float m_diagonalOffSetMultipler = 15.0f;

    private Vector3 m_mousePressedPosition;
    private bool m_leftButtonHeld = false;
    private Camera m_camera = null;
    private float m_currentMagnification = 0.0f;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
        Assert.IsNotNull(m_selectionBoxPrefab);
    }

    private void Update()
    {
        Move();
        onLeftClick();
        onRightClick();
        onScroll();
    }

    private void Move()
    {
        Vector3 position = new Vector3();

        //Diagonal
        if (Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x >= Screen.width - m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x + m_speed, 0, position.z + m_speed);
            position += move.normalized * m_speed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x >= Screen.width - m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x + m_speed, 0, position.z - m_speed);
            position += move.normalized * m_speed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x <= m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x - m_speed, 0, position.z - m_speed);
            position += move.normalized * m_speed * Time.deltaTime;
        }
        else if (Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x <= m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x - m_speed, 0, position.z + m_speed);
            position += move.normalized * m_speed * Time.deltaTime;
        }

        //Vertical
        else if (Input.mousePosition.y >= Screen.height - m_borderOffset)
        {
            position.z += m_speed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset)
        {
            position.z -= m_speed * Time.deltaTime;
        }

        //Horizontal
        else if (Input.mousePosition.x <= m_borderOffset)
        {
            position.x -= m_speed * Time.deltaTime;
        }
        else if (Input.mousePosition.x >= Screen.width - m_borderOffset)
        {
            position.x += m_speed * Time.deltaTime;
        }

        transform.position += position;
    }

    private void onLeftClick()
    {
        if (Input.GetMouseButtonDown(0) && !m_leftButtonHeld)
        {
            FactionPlayer.Instance.deselectAllUnits();

            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 10;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask) && hit.collider.tag == "Ground")
            {
                m_mousePressedPosition = hit.point;
            }
            
            m_leftButtonHeld = true;
            m_selectionBoxClone = Instantiate(m_selectionBoxPrefab, hit.point, Quaternion.identity);
            m_selectionBoxClone.transform.position = m_mousePressedPosition;
        }

        else if (m_leftButtonHeld)
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 10;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask) && hit.collider.tag == "Ground")
            {
                Assert.IsNotNull(m_selectionBoxClone);
                m_selectionBoxClone.transform.localScale = hit.point - m_mousePressedPosition;
                m_selectionBoxClone.transform.localScale =
                    new Vector3(m_selectionBoxClone.transform.localScale.x, m_selectionBoxHeight, m_selectionBoxClone.transform.localScale.z);
                m_selectionBoxClone.transform.position = m_mousePressedPosition + (hit.point - m_mousePressedPosition) / 2.0f;

                fRectangle selectionBoxAABB = new fRectangle(m_selectionBoxClone.transform.position, m_selectionBoxClone.transform.localScale);
                FactionPlayer.Instance.selectUnits(selectionBoxAABB);
            }

            if (Input.GetMouseButtonUp(0))
            {
                clearSelectionBox();
            }
        }
    }

    private void onRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Enemy")
            {
                FactionPlayer.Instance.targetEnemyAtPosition(hit.point);
            }
            else if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ground")
            {
                FactionPlayer.Instance.moveSelectedUnitsToPosition(hit.point);
                clearSelectionBox();
            }
        }
    }

    private void onScroll()
    {
        if(Input.mouseScrollDelta.y > 0.0f)
        {
            Vector3 newPosition = transform.position + transform.forward * m_zoomSpeed;
            Vector3 vBetween = newPosition - transform.position;
            if(m_currentMagnification * m_currentMagnification + vBetween.sqrMagnitude < m_maximumMagnification * m_maximumMagnification)
            {
                m_currentMagnification += vBetween.sqrMagnitude;
                transform.position = newPosition;
            }
        }
        else if(Input.mouseScrollDelta.y < 0.0f)
        {
            Vector3 newPosition = transform.position - transform.forward * m_zoomSpeed;
            Vector3 vBetween = newPosition - transform.position;
            if (m_currentMagnification * m_currentMagnification - vBetween.sqrMagnitude > 0)
            {
                m_currentMagnification -= vBetween.sqrMagnitude;
                transform.position = newPosition;
            }
        }
    }

    private void clearSelectionBox()
    {
        m_leftButtonHeld = false;

        if(m_selectionBoxClone)
        {
            Destroy(m_selectionBoxClone);
        }
    }
}