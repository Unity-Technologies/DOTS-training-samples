using Unity.Entities;
using Unity.Transforms;

public readonly partial struct PlayerAspect : IAspect
{
    public readonly RefRO<Player> Player;
    public readonly RefRW<LocalToWorldTransform> Transform;
}
