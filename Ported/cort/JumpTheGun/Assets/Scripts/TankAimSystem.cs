using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace JumpTheGun
{
    [UpdateAfter(typeof(PlayerBounceSystem))]
    public class TankAimSystem : JobComponentSystem
    {
        [BurstCompile]
        struct RotateTankBaseJob : IJobForEach<Translation, Rotation, TankBaseTag>
        {
            public Entity PlayerEntity;
            [ReadOnly] public ComponentDataFromEntity<Translation> PositionFromEntity;
            
            public void Execute([ReadOnly] ref Translation position, ref Rotation rotation, [ReadOnly] ref TankBaseTag _)
            {
                float3 diff = PositionFromEntity[PlayerEntity].Value - position.Value;
                float azimuthAngle = math.atan2(diff.x, diff.z);
                rotation.Value = quaternion.RotateY(azimuthAngle);
            }
        }

        private EntityQuery _playerQuery;
        protected override void OnCreate()
        {
            _playerQuery = GetEntityQuery(
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<ArcState>(),
                ComponentType.Exclude<Scale>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new RotateTankBaseJob()
            {
                PlayerEntity = _playerQuery.GetSingletonEntity(),
                PositionFromEntity = GetComponentDataFromEntity<Translation>(true),
            }.Schedule(this, inputDeps);
        }
    }
}
