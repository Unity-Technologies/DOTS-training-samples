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
        PheromoneTex = new Texture2D(dimensions, dimensions, TextureFormat.R8, false);
        Instance.GetComponent<MeshRenderer>().material.mainTexture = PheromoneTex;
    }

    void Start()
    {
        Instance = this;
    }
}
