
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class TurretAimSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity player = GetSingletonEntity<Player>();
        float3 targetPosition = GetComponent<Translation>(player).Value;
        targetPosition.y += 3;

        Entities.
            WithAll<Turret>()
            .ForEach((ref Rotation rotation, in Translation turretPosition) =>
            {
                float3 diff = targetPosition - turretPosition.Value;
                float angle = math.atan2(diff.x, diff.z);
                float deg = math.degrees(angle);

                rotation.Value = quaternion.EulerXYZ(-0.5F, angle, 0F);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
