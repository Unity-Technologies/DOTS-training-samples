using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class TankAimerSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;

        Entities
            .WithAll<TankBase>()
            .ForEach((ref Rotation rotation, ref Translation translation) =>
            {
                var player = GetSingletonEntity<Player>();
                var playerTranslation = GetComponent<Translation>(player);
                // Get an angle
                // quaternion yRot = Quaternion.LookRotation(mousePos, Vector3.up);
                var relPos = objPos - translation.Value;
                
                quaternion lookAtQuat = Quaternion.LookRotation(relPos, Vector3.right);
                // SetComponent the rotation
                rotation.Value = lookAtQuat;

            }).ScheduleParallel();
    }
}