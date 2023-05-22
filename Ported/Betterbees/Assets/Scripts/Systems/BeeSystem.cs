//Bee-haviour

using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct BeeSystem : ISystem
{
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {



        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
                var halfFloat = new float3(.5f, .5f, .5f);
                var random = new Random((uint)(state.WorldUnmanaged.Time.ElapsedTime * 1000) + 1);
                var delta = SystemAPI.Time.DeltaTime * 10f;
                foreach (var (beeState, transform) in SystemAPI.Query<RefRW<BeeState>, RefRW<LocalTransform>>())
                {
                        transform.ValueRW.Position += (random.NextFloat3() - halfFloat) * delta;
                }
        }
}