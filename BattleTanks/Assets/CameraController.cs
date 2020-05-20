using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private FactionPlayer m_player = null;
    [SerializeField]
    private GameObject m_selectionBoxPrefab = null;
    [SerializeField]
    private float m_magnificationSpeed = 0.0f;
    [SerializeField]
    private float m_maximumMagnification = 0.0f;
    [SerializeField]
    private float m_selectionBoxHeight = 0.5f;
    [SerializeField]
    private float m_movementSpeed = 30.0f;
    [SerializeField]
    private float m_borderOffset = 20.0f;
    [SerializeField]
    private float m_diagonalOffSetMultipler = 15.0f;
    [SerializeField]
    private float m_maxDistanceFromMap = 0.0f;
    [SerializeField]
    private bool m_movableByMouse = true;

    private GameObject m_selectionBoxClone = null;
    private Vector3 m_mousePressedPosition;
    private bool m_leftButtonHeld = false;
    private Camera m_camera = null;
    private float m_currentMagnification = 0.0f;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();

        Assert.IsNotNull(m_selectionBoxPrefab);
        Assert.IsNotNull(m_player);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (m_movableByMouse)
        {
            MoveByMouse();
        }

        moveByArrowKeys();

        onLeftClick();
        onRightClick();
        onScroll();

        if(Input.GetKeyDown(KeyCode.F))
        {
            m_player.spawnUnit(eUnitType.Attacker);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            m_player.spawnUnit(eUnitType.Harvester);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            m_player.setAttackMove(true);
        }
    }

    private void moveByArrowKeys()
    {
        Vector3 position = new Vector3();

        //Diagonal
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 move = new Vector3(position.x + m_movementSpeed, 0, position.z + m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 move = new Vector3(position.x + m_movementSpeed, 0, position.z - m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 move = new Vector3(position.x - m_movementSpeed, 0, position.z - m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 move = new Vector3(position.x - m_movementSpeed, 0, position.z + m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }

        //Vertical
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            position.z += m_movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            position.z -= m_movementSpeed * Time.deltaTime;
        }

        //Horizontal
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= m_movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            position.x += m_movementSpeed * Time.deltaTime;
        }

        if (isInBounds(Utilities.convertToGridPosition(transform.position + position)))
        {
            transform.position += position;
        }
    }

    private void MoveByMouse()
    {
        Vector3 position = new Vector3();

        //Diagonal
        if (Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x >= Screen.width - m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x + m_movementSpeed, 0, position.z + m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x >= Screen.width - m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x + m_movementSpeed, 0, position.z - m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x <= m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x - m_movementSpeed, 0, position.z - m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x <= m_borderOffset * m_diagonalOffSetMultipler)
        {
            Vector3 move = new Vector3(position.x - m_movementSpeed, 0, position.z + m_movementSpeed);
            position += move.normalized * m_movementSpeed * Time.deltaTime;
        }

        //Vertical
        else if (Input.mousePosition.y >= Screen.height - m_borderOffset)
        {
            position.z += m_movementSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset)
        {
            position.z -= m_movementSpeed * Time.deltaTime;
        }

        //Horizontal
        else if (Input.mousePosition.x <= m_borderOffset)
        {
            position.x -= m_movementSpeed * Time.deltaTime;
        }
        else if (Input.mousePosition.x >= Screen.width - m_borderOffset)
        {
            position.x += m_movementSpeed * Time.deltaTime;
        }

        if (isInBounds(Utilities.convertToGridPosition(transform.position + position)))
        {
            transform.position += position;
        }
    }

    private bool isInBounds(Vector2Int positionOnGrid)
    {
        //Horizontal
        if (positionOnGrid.x - m_camera.fieldOfView < 0)
        {
            Vector2Int positionOnBoundary = new Vector2Int(0, positionOnGrid.y);
            if((positionOnBoundary - 
                new Vector2Int(positionOnGrid.x - (int)m_camera.fieldOfView, positionOnGrid.y)).sqrMagnitude > m_maxDistanceFromMap * m_maxDistanceFromMap)
            {
                return false;
            }
        }
        else if (positionOnGrid.x + m_camera.fieldOfView > Map.Instance.m_mapSize.x)
        {
            Vector2Int positionOnBoundary = new Vector2Int(Map.Instance.m_mapSize.x, positionOnGrid.y);
            if ((positionOnBoundary -
                new Vector2Int(positionOnGrid.x + (int)m_camera.fieldOfView, positionOnGrid.y)).sqrMagnitude > m_maxDistanceFromMap * m_maxDistanceFromMap)
            {
                return false;
            }
        }
        
        //Vertical
        if (positionOnGrid.y < 0)
        {
            Vector2Int positionOnBoundary = new Vector2Int(positionOnGrid.x, 0);
            if ((positionOnBoundary - positionOnGrid).sqrMagnitude > m_maxDistanceFromMap * m_maxDistanceFromMap)
            {
                return false;
            }
        }
        else if(positionOnGrid.y > Map.Instance.m_mapSize.y)
        {
            Vector2Int positionOnBoundary = new Vector2Int(positionOnGrid.x, Map.Instance.m_mapSize.y);
            if((positionOnBoundary - positionOnGrid).sqrMagnitude > m_maxDistanceFromMap * m_maxDistanceFromMap)
            {
                return false;
            }
        }

        return true;
    }

    private void onLeftClick()
    {
        if (Input.GetMouseButtonDown(0) && !m_leftButtonHeld)
        {
            m_player.deselectAllUnits();

            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 10)) && hit.collider.tag == "Ground")
            {
                m_mousePressedPosition = hit.point;
            }
            else if(Physics.Raycast(ray, out hit) && hit.collider.tag == "PlayerBuilding")
            {
                m_player.selectBuilding(hit.point);
                return;
            }
            
            m_leftButtonHeld = true;
            m_selectionBoxClone = Instantiate(m_selectionBoxPrefab, hit.point, Quaternion.identity);
            m_selectionBoxClone.transform.position = m_mousePressedPosition;
        }

        else if (m_leftButtonHeld)
        {
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 10)) && hit.collider.tag == "Ground")
            {
                Assert.IsNotNull(m_selectionBoxClone);
                m_selectionBoxClone.transform.localScale = hit.point - m_mousePressedPosition;
                m_selectionBoxClone.transform.localScale =
                    new Vector3(m_selectionBoxClone.transform.localScale.x, m_selectionBoxHeight, m_selectionBoxClone.transform.localScale.z);
                m_selectionBoxClone.transform.position = m_mousePressedPosition + (hit.point - m_mousePressedPosition) / 2.0f;

                fRectangle selectionBoxAABB = new fRectangle(m_selectionBoxClone.transform.position, m_selectionBoxClone.transform.localScale);
                m_player.selectUnits(selectionBoxAABB);
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
            Physics.Raycast(ray, out hit);

            if (hit.collider.tag == "Enemy")
            {
                m_player.targetEnemyAtPosition(hit.point);
            }
            else if (hit.collider.tag == "Ground" || hit.collider.tag == "PlayerBuilding" || hit.collider.tag == "Resource")
            {
                m_player.handleSelectedUnits(hit.point);
            }

            clearSelectionBox();
        }
    }

    private void onScroll()
    {
        if(Input.mouseScrollDelta.y > 0.0f)
        {
            Vector3 newPosition = transform.position + transform.forward * m_magnificationSpeed;
            Vector3 vBetween = newPosition - transform.position;
            if(m_currentMagnification * m_currentMagnification + vBetween.sqrMagnitude < m_maximumMagnification * m_maximumMagnification)
            {
                m_currentMagnification += vBetween.sqrMagnitude;
                transform.position = newPosition;
            }
        }
        else if(Input.mouseScrollDelta.y < 0.0f)
        {
            Vector3 newPosition = transform.position - transform.forward * m_magnificationSpeed;
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
        m_player.setAttackMove(false);

        if(m_selectionBoxClone)
        {
            Destroy(m_selectionBoxClone);
        }
    }
}