using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField]
    int m_TargetFrameRate = 60;
    
    void Start()
    {
        Application.targetFrameRate = m_TargetFrameRate;
    }
}
