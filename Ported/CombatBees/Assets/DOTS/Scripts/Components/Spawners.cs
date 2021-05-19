using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public struct BeeSpawner : IComponentData 
{
    public Entity BeePrefab;
    public int BeeCount;
    public int BeeCountFromResource;

    public int MaxSpeed;
    public int MaxSize;
}

public struct ResourceSpawner : IComponentData 
{
    public Entity ResourcePrefab;
    public int ResourceCount;
}