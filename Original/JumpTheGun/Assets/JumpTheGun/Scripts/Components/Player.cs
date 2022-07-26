using Unity.Entities;
using Unity.Mathematics;

struct Player : IComponentData
{
    public bool playerState;
    public float bounceDuration;
    public float yOffset;
    public float bounceHeight; 
    public float time;
    public float duration;
    public Para para; 
}