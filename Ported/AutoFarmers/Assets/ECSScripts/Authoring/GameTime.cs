using Unity.Entities;
using Unity.Mathematics;

public struct GameTime : IComponentData
{
    public float DeltaTime;
    public double ElapsedTime;
}
