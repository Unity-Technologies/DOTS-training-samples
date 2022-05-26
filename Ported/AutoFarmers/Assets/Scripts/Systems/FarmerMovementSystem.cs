using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
/*
[BurstCompile]
public partial class FarmerMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<GameConfig>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        //var config = SystemAPI.GetSingleton<GameConfig>();

        //var dt = Time.DeltaTime;
        //float movementSpeed = dt * config.FarmerMoveSpeed;

        //Entities
        //    .WithAll<Farmer>()
        //    .ForEach((Entity entity, TransformAspect transform, Targeting targeting) =>
        //    {
        //        if( targeting.entityTarget == Entity.Null )
        //        {
        //            // DEBUG: with no target keep moving arounnd randomly - remove when all movement is sorted
        //            var pos = transform.Position;
        //            pos.y = entity.Index;
        //            var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
        //            var dir = float3.zero;
        //            math.sincos(angle, out dir.x, out dir.z);
        //            transform.Position += dir * movementSpeed;
        //            transform.Rotation = quaternion.RotateY(angle);
        //        }
        //        else
        //        {
        //            float3 targetPosition = new float3(
        //                targeting.tileTarget.x,
        //                0,
        //                targeting.tileTarget.y);

        //            if(math.distancesq(targetPosition, transform.Position) > 0.1f * 0.1f)
        //            {
        //                float3 farmerToTarget = targetPosition - transform.Position;
        //                float3 unitVelocity = math.normalize(farmerToTarget);
        //                transform.Position += unitVelocity * movementSpeed;
        //            }
        //            else
        //            {
        //                transform.Position = targetPosition;
        //            }
        //        }
        //    }).ScheduleParallel();
    }
}
*/