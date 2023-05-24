using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneTextureView : MonoBehaviour
{
    public static Texture2D PheromoneTex;
    public static PheromoneTextureView Instance;

    public static void Initialize(int dimensions)
    {
        PheromoneTex = new Texture2D(dimensions, dimensions, TextureFormat.R16, false);
        Instance.GetComponent<MeshRenderer>().material.mainTexture = PheromoneTex;
    
        Instance.transform.localScale = Vector3.one * dimensions * 0.1f;
        Instance.transform.position = new Vector3(dimensions * 0.5f, 0f, dimensions * 0.5f);
    }

    void Start()
    {
        Instance = this;
    }
}
