using System;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject turret;
    public Vector3 worldSpaceTarget;
    public float turretTurnSpeed = 0.1f;

    float currentFacing = 0f;

    float shortestRotationBetween(float Target, float Start)
    {
        float tryA = Target - Start;
        float tryB = Target - Start + (2 * (float)Math.PI);
        float tryC = Target - Start - (2 * (float)Math.PI);

        if (Math.Abs(tryA) < Math.Abs(tryB) && Math.Abs(tryA) < Math.Abs(tryC))      //A is smallest
            return tryA;
        else if (Math.Abs(tryB) < Math.Abs(tryA) && Math.Abs(tryB) < Math.Abs(tryC)) //B is smallest
            return tryB;
        else                                                                         //C is smallest
            return tryC;
    }

    // Start is called before the first frame update
    void Start()
    {
        worldSpaceTarget = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //Find relative angle from forward vector to target
        Vector3 vecA = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 vecB = new Vector3(worldSpaceTarget.x - transform.position.x, 0, worldSpaceTarget.z - transform.position.z);
        float targetAngle = (float)Math.Acos(Vector3.Dot(vecA, vecB) / (vecA.magnitude * vecB.magnitude));

        //if difference is less than turn speed then set angle to that
        if (Math.Abs(currentFacing - targetAngle) < turretTurnSpeed)
        {
            currentFacing = targetAngle;
            turret.transform.LookAt(worldSpaceTarget);
        }
        //Otherwise move turnspeed towards that direction
        else if (shortestRotationBetween(targetAngle, currentFacing) < 0f)
        {
            currentFacing -= turretTurnSpeed;
            turret.transform.Rotate(new Vector3(0, turretTurnSpeed, 0));
        }
        else
        {
            currentFacing += turretTurnSpeed;
            turret.transform.Rotate(new Vector3(0, turretTurnSpeed, 0));
        }
        


        //Old attempt
        //Convert world space target to local space
        //Vector3 rotateTo = transform.worldToLocalMatrix.MultiplyVector(worldSpaceTarget);
        //rotateTo.z = 0;
        //Quaternion q = Quaternion.LookRotation(rotateTo);
        //turret.transform.rotation = q;
    }
}
