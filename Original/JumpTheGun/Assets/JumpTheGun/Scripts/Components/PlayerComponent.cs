using Unity.Entities;
using Unity.Mathematics;

struct PlayerComponent : IComponentData
{
    public bool isBouncing;
    
    public float bounceDuration;
    public float yOffset;
    public float bounceHeight; 
    public float time;
    public float duration;
    public Para para; 

    public Entity startBox;
	public Entity endBox;
}