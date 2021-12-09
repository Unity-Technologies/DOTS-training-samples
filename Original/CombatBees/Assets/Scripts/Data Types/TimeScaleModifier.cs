using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleModifier : MonoBehaviour
{
    // Start is called before the first frame update
   
    // Update is called once per frame
    public float slowMoSpeed=0.1f;
    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            Time.timeScale = slowMoSpeed;
           
        }

        else
        {
            Time.timeScale = 1;
        }
    }
}
