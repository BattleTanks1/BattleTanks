using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple class to allow all unit movement to be resolved after movement calculation is completed each frame
public class UnitResolveMove : MonoBehaviour
{
    private UnitMovement moveClass;

    private void Start()
    {
        moveClass = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        transform.position += moveClass.m_velocity * Time.deltaTime;
    }
}
