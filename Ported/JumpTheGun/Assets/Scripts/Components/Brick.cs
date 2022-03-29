using Unity.Entities;
using Color = UnityEngine.Color;

public struct Brick : IComponentData
{
    public Color color;
    public float height;
}



