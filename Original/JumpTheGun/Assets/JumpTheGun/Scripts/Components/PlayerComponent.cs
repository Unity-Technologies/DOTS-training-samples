using Unity.Entities;
using Unity.Mathematics;

struct PlayerComponent : IComponentData
{
    public Entity playerEntity;
    public bool playerState;
    public float bounceDuration;
    public float yOffset;
    public float bounceHeight; 
    public float time;
    public float duration;
    public Para para; 
}