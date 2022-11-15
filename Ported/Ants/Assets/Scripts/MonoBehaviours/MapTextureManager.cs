using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MapTextureManager : MonoBehaviour
{
    private Material material;
    private MeshRenderer meshRenderer;
    
    private Texture2D texture;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        texture = new Texture2D(1024, 1024, TextureFormat.R8, 0, false);
        material = meshRenderer.material;
        material.mainTexture = texture;
    }
    
    public void SetTextureData(NativeArray<byte> data)
    {
        texture.SetPixelData(data, 0);
        texture.Apply(true);
        material.mainTexture = texture;
    }
}
