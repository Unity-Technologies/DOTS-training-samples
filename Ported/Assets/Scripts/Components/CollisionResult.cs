using Unity.Entities;
using UnityMath = Unity.Mathematics;

public struct CollisionResult : IComponentData
{
    public UnityMath.float2 point;
    public UnityMath.float2 normal;

    public readonly bool IsValid => float.IsNaN(point.x);

    public CollisionResult(in float pointX = float.NaN, in float pointY = float.NaN, in float normalX = float.NaN, in float normalY = float.NaN)
    {
        this.point = new UnityMath.float2(pointX, pointY);
        this.normal = UnityMath.math.normalize(new UnityMath.float2(normalX, normalY));
    }

    public void Reset() => point.x = float.NaN;
}
