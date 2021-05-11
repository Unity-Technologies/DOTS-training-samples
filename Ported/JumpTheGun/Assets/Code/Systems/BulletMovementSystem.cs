using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class BulletMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //Entities.WithAll<Bullet, Arc, Direction>().WithAll<Time, TimeOffset>
    }
}
