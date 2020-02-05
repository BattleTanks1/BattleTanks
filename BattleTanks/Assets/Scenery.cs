using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenery : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        Rectangle rect = new Rectangle((int)transform.position.x - (int)transform.localScale.x, 
            (int)transform.position.x + (int)transform.localScale.x, 
            (int)transform.position.z - (int)transform.localScale.z,
            (int)transform.position.z + (int)transform.localScale.z);

        fGameManager.Instance.addScenery(rect);
    }
}