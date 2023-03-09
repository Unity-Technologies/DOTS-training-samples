using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PheromoneRenderer : MonoBehaviour
{
    public const int MAP_WIDTH = 256;
    public const int MAP_HEIGHT = 256;
    
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
    
    public void SetTextureData(NativeArray<float> data)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < data.Length; i++)
        {
            pixels[i] = new Color(data[i], 0.5f,0.5f);
        }
        texture.SetPixels(pixels);
        texture.Apply(true);
    }
}




