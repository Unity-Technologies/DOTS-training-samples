using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

class WaterCellAuthoring : UnityEngine.MonoBehaviour
{
    public float Volume = 0.0f;
}

class WaterCellBaker : Baker<WaterCellAuthoring>
{
    public override void Bake(WaterCellAuthoring authoring)
    {
        float2 wsPosition = new float2(authoring.transform.position.x, authoring.transform.position.z);
        AddComponent(new Volume { Value = authoring.Volume });
        AddComponent(new Position { Value = wsPosition });
    }
}
