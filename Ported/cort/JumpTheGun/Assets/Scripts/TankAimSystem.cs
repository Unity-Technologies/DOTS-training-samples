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
    [UpdateAfter(typeof(PlayerPositionCacheSystem))]
    public class TankAimSystem : JobComponentSystem
    {
        [BurstCompile]
        struct RotateTankBaseJob : IJobForEach<Translation, Rotation, TankBaseTag>
        {
            public float3 PlayerPosition;
            
            public void Execute([ReadOnly] ref Translation position, ref Rotation rotation, [ReadOnly] ref TankBaseTag _)
            {
                float3 diff = PlayerPosition - position.Value;
                float azimuthAngle = math.atan2(diff.x, diff.z);
                rotation.Value = quaternion.RotateY(azimuthAngle);
            }
        }

        private PlayerPositionCacheSystem _playerPosCache;
        protected override void OnCreateManager()
        {
            _playerPosCache = World.GetOrCreateSystem<PlayerPositionCacheSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new RotateTankBaseJob()
            {
                PlayerPosition = _playerPosCache.PlayerPosition,
            }.Schedule(this, inputDeps);
        }
    }
}
