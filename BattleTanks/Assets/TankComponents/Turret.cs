using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject turret;
    public Vector3 worldSpaceTarget;
    Vector3 currentFacing;
    float turretTurnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        worldSpaceTarget = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //Convert world space target to local space
        Vector3 rotateTo = transform.worldToLocalMatrix.MultiplyVector(worldSpaceTarget);
        rotateTo.z = 0;
        Quaternion q = Quaternion.LookRotation(rotateTo);
        turret.transform.rotation = q;
    }
}
