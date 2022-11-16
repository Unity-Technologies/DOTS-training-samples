using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MapTextureManager : MonoBehaviour
{
    public const int MAP_WIDTH = 128;
    public const int MAP_HEIGHT = 128;
    
    private Material material;
    private MeshRenderer meshRenderer;
    
    private Texture2D texture;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        texture = new Texture2D(MAP_WIDTH, MAP_HEIGHT, TextureFormat.R8, 0, false);
        material = meshRenderer.material;
        material.mainTexture = texture;
    }
    
    public void SetTextureData(NativeArray<byte> data)
    {
        texture.SetPixelData(data, 0);
        texture.Apply(true);
    }
}
