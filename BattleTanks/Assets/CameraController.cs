
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_speed = 30.0f;
    public float m_borderOffset = 20.0f;
    public float m_diagonalOffSetMultipler = 15.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = new Vector3();   

        //Diagonal
        if(Input.mousePosition.y >= Screen.height - m_borderOffset * m_diagonalOffSetMultipler && 
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
        else if(Input.mousePosition.y >= Screen.height - m_borderOffset)
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
}
