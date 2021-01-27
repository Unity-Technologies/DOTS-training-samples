using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheadCamera : MonoBehaviour
{
    public Slider TrackSizeSlider;

    private float Aspect = Mathf.PI / 4.0f;

    public float GetHeight(float size)
    {
        float ratio = Mathf.Sqrt(3) + 1 / Mathf.Sqrt(3) - 1;
        return ratio * size * 0.9f;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        pos.y = GetHeight(TrackSizeSlider.value);
        pos.z = GetHeight(TrackSizeSlider.value) / 4;
        transform.position = pos;
        transform.LookAt(Vector3.zero);
    }
}
