using Unity.Entities;
using Unity.Mathematics;
public struct Turret : IComponentData
{
    public Entity cannonBall;
    public Entity cannonBallSpawn;

}
