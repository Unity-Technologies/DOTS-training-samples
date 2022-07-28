using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BlueBeeAuthoring : MonoBehaviour
{
    
}

class BlueBeeBaker : Baker<BlueBeeAuthoring>
{
    public override void Bake(BlueBeeAuthoring authoring)
    {
        AddComponent(new Bee
        {
            target = Entity.Null,
            state = BeeState.Idle
        });
        AddComponent(new NonUniformScale
        {
            Value = new float3(1.01f, 1.01f, 1.01f)
        });
        AddComponent<BlueTeam>();
    }
}
