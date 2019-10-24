using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Overlays : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject OverlayPrefab;
    public GameObject[] Overlay => m_Overlays;

    private const int k_PlayerCount = 4;
    private GameObject[] m_Overlays;

    private void OnEnable()
    {
        if (OverlayPrefab != null && m_Overlays != null)
            return;
        m_Overlays = new GameObject[4*3];
        foreach(var go in m_Overlays)
            DestroyImmediate(go);
        for (int i = 0; i < 4 * 3; ++i)
        {
            var instance = Instantiate(OverlayPrefab);
            instance.GetComponent<Transform>().position = new Vector3(0,-10,0);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //
    }
}
