using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class AntMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(4567);
        var time = Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((ref Translation translation, ref Rotation rotation, in AntMovement ant, in LocalToWorld ltw) =>
            {
                Quaternion _rotation = Quaternion.Euler(0, random.NextFloat(-Config.RotationAngle, Config.RotationAngle), 0);
                float3 _forward = _rotation * ltw.Forward;
                //_forward.y += random.NextFloat(-Config.RotationAngle, Config.RotationAngle);
                
                translation.Value += _forward * Config.MoveSpeed * (float) time;
                rotation.Value = Quaternion.Euler(_forward);
                //translation.Value.x = (float)((time + Config.MoveSpeed) % 100) - 50f;
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
