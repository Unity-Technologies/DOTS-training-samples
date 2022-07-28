using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnFlashAuthoring : MonoBehaviour
{
    
}

class SpawnFlashBaker : Baker<SpawnFlashAuthoring>
{
    public override void Bake(SpawnFlashAuthoring authoring)
    {
        AddComponent<SpawnFlash>();
        AddComponent(new NonUniformScale
        {
            Value = new float3(0.1f, 0.1f, 0.1f)
        });
    }
}