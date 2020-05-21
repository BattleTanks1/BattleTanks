using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextResourceDisplay : MonoBehaviour
{
    Text thisText;

    // Start is called before the first frame update
    void Start()
    {
        thisText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        thisText.text = "Resources: " + GameManager.Instance.getPlayerResources();
    }
}
