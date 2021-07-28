using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class TankAimerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        var player = GetSingletonEntity<Player>();
        var playerTranslation = GetComponent<Translation>(player);

        Entities
            .WithAll<LookAtPlayer>()
            .ForEach((ref Rotation rotation, ref Translation translation) =>
            {
                
                
                // Get an angle
                // quaternion yRot = Quaternion.LookRotation(mousePos, Vector3.up);
                var forward = playerTranslation.Value - translation.Value;
                forward.y = 0f;
                
                quaternion lookAtQuat = quaternion.LookRotation(forward, new float3(0f,1f,0));
                //rotation.Value = math.normalize(new quaternion(0f, lookAtQuat.value.y, 0f, lookAtQuat.value.w));
                
                rotation.Value = lookAtQuat;
                // SetComponent the rotation
                // rotation.Value = new quaternion(0f, lookAtQuat.value.y, 0f, lookAtQuat.value.w);

            }).ScheduleParallel();
    }
}