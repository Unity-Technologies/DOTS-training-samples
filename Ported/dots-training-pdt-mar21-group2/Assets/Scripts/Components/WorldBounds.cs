using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WorldBounds : IComponentData
{
    public float Width;
    public float Ground;

    public bool IsOutOfBounds(float3 position)
    {
        return position.x < 0.0f ||
               position.x > Width ||
               position.y < Ground;
    }
}
