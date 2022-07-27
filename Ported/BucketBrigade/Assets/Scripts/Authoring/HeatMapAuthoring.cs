using Unity.Entities;

public class HeatMapAuthoring : UnityEngine.MonoBehaviour
{
}

public class HeatMapBaker: Baker<HeatMapAuthoring>
{
    public override void Bake(HeatMapAuthoring authoring)
    {
        AddBuffer<Temperature>();
    }
}
