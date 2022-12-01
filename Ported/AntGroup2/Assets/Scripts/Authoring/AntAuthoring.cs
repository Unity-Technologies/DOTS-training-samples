using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class AntAuthoring : UnityEngine.MonoBehaviour
{
    
}

class AntBaker : Baker<AntAuthoring>
{
    public override void Bake(AntAuthoring authoring)
    {
        AddComponent<Ant>();
        AddComponent(new TargetDirection() { Angle = 0 });
        AddComponent(new PheromoneDirection() { Angle = 0 });
        AddComponent(new WallDirection() { Angle = 0 });
        AddComponent(new HasResource() { Value = false });
        AddComponent(new URPMaterialPropertyBaseColor() { Value = new float4(1.0f, 1.0f, 1.0f, 1.0f) });
        AddComponent(new CurrentDirection { Angle = 0 });
        AddComponent(new PreviousDirection { Angle = 0 });
    }
}
