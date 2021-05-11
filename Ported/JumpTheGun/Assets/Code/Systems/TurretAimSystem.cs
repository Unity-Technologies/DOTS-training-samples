
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TurretAimSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity player = GetSingletonEntity<Player>();
        float3 targetPosition = GetComponent<Translation>(player).Value;
        targetPosition.y += 3;

        Entities.
            WithAll<Turret, Translation, Rotation>()
            .ForEach((ref Rotation rotation, in Translation turretPosition) =>
            {
                rotation.Value = quaternion.LookRotation(turretPosition.Value, targetPosition);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
