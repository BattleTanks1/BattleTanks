using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scenery : MonoBehaviour
{
    void Start()
    {
        Map.Instance.addScenery(new iRectangle(transform.position, transform.localScale));
    }
}