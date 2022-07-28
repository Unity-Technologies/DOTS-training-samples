using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class YellowBeeAuthoring : MonoBehaviour
{
    
}

class YellowBeeBaker : Baker<YellowBeeAuthoring>
{
    public override void Bake(YellowBeeAuthoring authoring)
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
        AddComponent<YellowTeam>();
    }
}
