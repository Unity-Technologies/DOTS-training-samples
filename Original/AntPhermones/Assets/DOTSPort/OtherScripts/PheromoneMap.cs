using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneMap : MonoBehaviour
{
    Material m_PheromoneMaterial;
    Texture2D m_PheromoneMapTexture;
    
    public Texture2D PheromoneMapTexture => m_PheromoneMapTexture;

    void Awake()
    {
        m_PheromoneMaterial = GetComponent<MeshRenderer>().material;
    }

    public void CreateTexture(int width, int height)
    {
        transform.position = new Vector3(width / 2f, height/ 2f, 0.1f);
        transform.localScale = new Vector3(width, height, 1);
        m_PheromoneMapTexture = new Texture2D(width, height, TextureFormat.R8, false);
        var pixels = m_PheromoneMapTexture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        m_PheromoneMapTexture.SetPixels(pixels);
        m_PheromoneMapTexture.Apply();
        m_PheromoneMaterial.SetTexture("_BaseMap", m_PheromoneMapTexture);
    }
}
