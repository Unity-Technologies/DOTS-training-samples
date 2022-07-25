using Unity.Entities;
using Unity.Mathematics;

struct Player : IComponentData
{
    public float3 position; 
    public float3 rotation;
    public float3 scale;

    public float4 color;
    public bool playerState;
    public float bounceDuration;
    public float yOffset;
    public float bounceHeight; 
    public float time;
    public float duration;
    public Para para; 
}