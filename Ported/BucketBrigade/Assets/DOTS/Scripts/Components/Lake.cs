using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

[GenerateAuthoringComponent]
public struct Lake : IComponentData
{
    public float Volume;
}

public struct BucketFillAction : IBufferElementData
{
    public Entity Bucket;
    public Entity FireFighter;

    public float BucketVolume;
    public float3 Position;
}

public struct EmptyLake : IComponentData { }