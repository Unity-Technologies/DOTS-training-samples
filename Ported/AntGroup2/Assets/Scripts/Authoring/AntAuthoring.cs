using Unity.Entities;
using Unity.Mathematics;

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
    }
}
