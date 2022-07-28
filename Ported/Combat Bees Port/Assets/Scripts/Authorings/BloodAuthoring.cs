using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BloodAuthoring : MonoBehaviour
{
    
}

class BloodBaker : Baker<BloodAuthoring>
{
    public override void Bake(BloodAuthoring authoring)
    {
        AddComponent<Blood>();
        AddComponent(new NonUniformScale
        {
            Value = new float3(1f, 1f, 1f)
        });
    }
}