using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridPlane  : IDisposable
{
    public int Width;
    public int Height;

    public Texture2D Texture;

    private GameObject m_Plane;
    private Material m_GridMaterial;

    public GridPlane(int width, int height)
    {
        Width = width;
        Height = height;

        m_GridMaterial = new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ColorEditMaterial.mat"));

        Texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
        Texture.filterMode = FilterMode.Point;
        Texture.wrapMode = TextureWrapMode.Clamp;

        m_Plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        m_Plane.name = "FireGrid";

        m_Plane.GetComponent<Renderer>().sharedMaterial = m_GridMaterial;
        m_Plane.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", Texture);

        m_Plane.transform.position = new Vector3(width / 2, 0, height / 2);
        m_Plane.transform.localScale = new Vector3(width / 10, 1, height / 10);
        m_Plane.transform.rotation = Quaternion.Euler(0, 180, 0);
        m_Plane.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Dispose()
    {
        Object.DestroyImmediate(m_Plane);
    }
}