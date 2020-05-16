using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Critters : MonoBehaviour
{
    private List<GameObject> m_critters = null;

    private void Awake()
    {
        m_critters = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void initialize(GameObject critterPrefab)
    {
        Assert.IsTrue(m_critters.Count == 0);
        Assert.IsNotNull(critterPrefab);

        int critterCount = Random.Range(GameConstants.MIN_CRITTER_COUNT, GameConstants.MAX_CRITTER_COUNT);
        for(int i = 0; i < critterCount; ++i)
        {
            Instantiate()
        }
    }
}