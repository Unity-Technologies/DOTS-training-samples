using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileComponentData: IComponentData
{
    public float radius;
    public float velocityX;
    public float rangeXMin;
    public float rangeXMax;
}
