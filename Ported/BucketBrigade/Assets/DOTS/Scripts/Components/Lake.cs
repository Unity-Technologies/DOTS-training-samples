using Unity.Entities;
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
}
