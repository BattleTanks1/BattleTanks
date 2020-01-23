using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    Vector3 worldSpaceTarget;
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
        //Rotate some amount toward target based on turret turn speed
    }
}
