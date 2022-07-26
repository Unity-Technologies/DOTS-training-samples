using Unity.Entities;

class PlayerAuthoring : UnityEngine.MonoBehaviour
{
    public float bounceDuration;
    public float bounceHeight; 
    public float yOffset;
    public float duration;
}

class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        AddComponent(new Player
        {
            bounceDuration = authoring.bounceDuration,
            bounceHeight = authoring.bounceHeight, 
            yOffset = authoring.yOffset,
            duration = authoring.duration,
        });
    }
}