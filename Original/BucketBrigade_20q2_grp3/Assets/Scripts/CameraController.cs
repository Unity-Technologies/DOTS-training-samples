using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Look at the center of the grid
    // TODO would be nice to position camera based on grid size
    void Start()
    {
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        var t = cam.transform;
        Bootstrap bs = GameObject.Find("Bootstrap").GetComponent<Bootstrap>();
        t.LookAt(new Vector3(bs.GridWidth/2,0,bs.GridHeight/2)); 
    }

    // Update is called once per frame
    void Update()
    {
    }
}
