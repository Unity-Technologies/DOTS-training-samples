using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[GenerateAuthoringComponent]
public struct RenderSpawner : IComponentData
{
    public Entity rockPrefab;
    public Entity plantPrefab;
    public Entity shopPrefab;
    public Entity landPrefab;
    public Entity tilledPrefab;
    
    public float TileSize;
}
