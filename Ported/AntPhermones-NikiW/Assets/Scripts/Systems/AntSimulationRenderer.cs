using System;
using UnityEngine;

public class AntSimulationRenderer : MonoBehaviour
{
    public static AntSimulationRenderer Instance { get; private set; }
    
    public Material basePheromoneMaterial;
    public Color carryColor;
    public Material colonyMaterial; 
    public Mesh colonyMesh;
    public Material resourceMaterial;
    public Mesh resourceMesh;
    public Renderer textureRenderer;
    public Mesh antMesh;
    public Material antMaterial;
    public Mesh obstacleMesh;
    public Material obstacleMaterial;

    void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = default;
    }
}
