using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public int framerate = 30;
    void Start()
    {
        Application.targetFrameRate = framerate;
    }
}

