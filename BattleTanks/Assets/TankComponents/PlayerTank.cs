using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    Vector3 linePlaneIntersection(Vector3 linePos, Vector3 lineDir, Vector3 planePos, Vector3 planeNormal)
    {
        float lineDotNormal = Vector3.Dot(lineDir, planeNormal);
        if (lineDotNormal == 0)
            return new Vector3();

        float d = Vector3.Dot((planePos - linePos), planeNormal) / lineDotNormal;
        return linePos + (lineDir * d);
    }

    Vector3 mousePosToPlanePos(Vector3 worldPos, Vector3 planeNormal, Vector3 planePos)
    {
        Camera cam = Camera.main;
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        Vector3 pointA = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        Vector3 eyeToA = pointA - cam.transform.position;
        Vector3 mouseWorldPos = linePlaneIntersection(cam.transform.position, eyeToA, planePos, planeNormal);

        Vector3 vecBetween = worldPos - mouseWorldPos;
        return vecBetween;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TankCore>().m_faction = Faction.player;
    }

    // Update is called once per frame
    void Update()
    {
        TankMovement move = GetComponent<TankMovement>();
        //Rotation
        if (Input.GetKey(KeyCode.D))
        {
            move.leftTurn();
        }
        if(Input.GetKey(KeyCode.A))
        {
             move.rightTurn();
        }
        //Movement
        if(Input.GetKey(KeyCode.W))
        {
            move.forward();
        }
        if(Input.GetKey(KeyCode.S))
        {
            move.backward();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Shoot
        }

        //Vector3 turretDir = mousePosToPlanePos(transform.position, new Vector3(0, 1, 0), new Vector3(0, 0, 0));
        //GetComponent<Turret>().worldSpaceTarget = turretDir.normalized;
    }
}