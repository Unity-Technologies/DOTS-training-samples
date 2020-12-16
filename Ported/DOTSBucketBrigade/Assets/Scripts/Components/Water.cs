using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    public float volume = 10.0f;
    public float capacity = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        capacity = volume; 
    }

    private void Update()
    {
        if (volume < 0.5f)
            DepleteWaterSource();
    }
    public bool ScoopWater(float scoopAmount)
    {
        if (scoopAmount > volume)
            return false;

        volume -= scoopAmount;
        return true;
    }

    private void DepleteWaterSource()
    {
        Destroy(this.gameObject);
    }
}
