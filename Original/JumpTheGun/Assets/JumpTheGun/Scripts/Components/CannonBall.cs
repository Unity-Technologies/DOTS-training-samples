using Unity.Entities;
using Unity.Mathematics;
public struct CannonBall : IComponentData
{
    public float3 position;
   // public float3 rotation;
   // public float3 scale;
   // public float4 color;
   // public float3 startPos;
   // public float3 targetPos;

    public float speed;
    public float radius;
    public float duration;

    public Para para;



}
