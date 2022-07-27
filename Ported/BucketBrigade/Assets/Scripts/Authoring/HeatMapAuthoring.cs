using Unity.Entities;

class HeatMapAuthoring : UnityEngine.MonoBehaviour
{
}

class HeatMapBaker: Baker<HeatMapAuthoring>
{
    public override void Bake(HeatMapAuthoring authoring)
    {
        AddBuffer<Temperature>();
    }
}
