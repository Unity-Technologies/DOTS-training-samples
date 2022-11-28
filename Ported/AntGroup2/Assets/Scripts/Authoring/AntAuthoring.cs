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
        AddComponent(new TargetDirection() {Direction = new float2(1.0f, 0)});
    }
}
