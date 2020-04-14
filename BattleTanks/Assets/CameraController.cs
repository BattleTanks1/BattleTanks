using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    public GameObject m_selectionBoxPrefab = null;
    private GameObject m_selectionBoxClone = null;

    [SerializeField]
    private float m_selectionBoxHeight = 0.5f;
    [SerializeField]
    private float m_speed = 30.0f;
    [SerializeField]
    private float m_borderOffset = 20.0f;
    [SerializeField]
    private float m_diagonalOffSetMultipler = 15.0f;
    [SerializeField]
    private float m_minSelectionBoxSize = 0.0f;

    private Vector3 m_mousePressedPosition;
    private bool m_leftButtonHeld = false;
    private bool m_unitsSelected = false;
    private bool m_renderSelectionBox = false;
    private Camera m_camera = null;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    private void Update()
    {
        Move();
        handleSelectionBox();
    }

    private fRectangle getSelectionBox(Vector3 position, Vector3 localScale)
    {
        Vector3 scale = new Vector3(localScale.x / 2.0f, 0, localScale.z / 2.0f);

        return new fRectangle(Mathf.Min(position.x - scale.x, position.x + scale.x),
                Mathf.Max(position.x - scale.x, position.x + scale.x),
                Mathf.Min(position.z - scale.z, position.z + scale.z),
                Mathf.Max(position.z - scale.z, position.z + scale.z));
    }

    private void Move()
    {
        Vector3 position = new Vector3();

        //Diagonal
        if (Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x >= Screen.width - m_borderOffset * m_diagonalOffSetMultipler)
        {
            position.x += (m_speed / 2.0f) * Time.deltaTime;
            position.z += (m_speed / 2.0f) * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x >= Screen.width - m_borderOffset * m_diagonalOffSetMultipler)
        {
            position.x += (m_speed / 2.0f) * Time.deltaTime;
            position.z -= (m_speed / 2.0f) * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x <= m_borderOffset * m_diagonalOffSetMultipler)
        {
            position.x -= (m_speed / 2.0f) * Time.deltaTime;
            position.z -= (m_speed / 2.0f) * Time.deltaTime;
        }
        else if (Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler &&
            Input.mousePosition.x <= m_borderOffset * m_diagonalOffSetMultipler)
        {
            position.x -= (m_speed / 2.0f) * Time.deltaTime;
            position.z += (m_speed / 2.0f) * Time.deltaTime;
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

    private void handleSelectionBox()
    {
        if (Input.GetMouseButtonDown(0) && !m_leftButtonHeld)
        {
            GameManager.Instance.deselectPlayerUnits();

            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ground")
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
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ground")
            {
                Assert.IsNotNull(m_selectionBoxClone);
                m_selectionBoxClone.transform.localScale = hit.point - m_mousePressedPosition;
                m_selectionBoxClone.transform.position = m_mousePressedPosition + (hit.point - m_mousePressedPosition) / 2.0f;

                fRectangle selectionBoxAABB = getSelectionBox(m_selectionBoxClone.transform.position, m_selectionBoxClone.transform.localScale);
                m_selectionBoxClone.transform.localScale =
                    new Vector3(m_selectionBoxClone.transform.localScale.x, m_selectionBoxHeight, m_selectionBoxClone.transform.localScale.z);

                GameManager.Instance.selectPlayerUnits(selectionBoxAABB);
                
            }


            if (Input.GetMouseButtonUp(0))
            {
                m_renderSelectionBox = false;
                m_leftButtonHeld = false;
                Assert.IsNotNull(m_selectionBoxClone);
                Destroy(m_selectionBoxClone);
            }
        }
    }
}