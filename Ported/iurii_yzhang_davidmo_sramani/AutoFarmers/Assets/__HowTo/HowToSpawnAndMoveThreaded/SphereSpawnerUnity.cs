using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class SphereSpawnerUnity : MonoBehaviour
{
    public GameObject prefabObject;
    
    static SphereSpawnerUnity m_instance;
    public static SphereSpawnerUnity instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<SphereSpawnerUnity>();
            return m_instance;
        }
    }

    private void Awake()
    {
        m_instance = this;
    }
}
