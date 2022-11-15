using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [BurstCompile]
    partial struct SwarmJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Bee> BeeComponents;
        public float TeamAttraction;
        public float Dt;
        public uint Seed;

        void Execute([EntityInQueryIndex] int index, ref Bee currentBee)
        {
            var newRandom = Random.CreateFromIndex((uint)index + Seed);
            var randomBee = BeeComponents[newRandom.NextInt(BeeComponents.Length)];

            float3 delta = randomBee.Position - currentBee.Position;
            float dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f)
            {
                var velocity = currentBee.Velocity;
                velocity += delta * (TeamAttraction * Dt / dist);
                currentBee.Velocity = velocity;
            }
        }
    }
    
    [BurstCompile]
    public partial struct BeeSwarmingSystem : ISystem
    {
        private Random _random;
        private uint _randomSeed;
        private EntityQuery _team1Bees;
        private EntityQuery _team2Bees;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Bee>();
            
            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<Bee, TeamIdentifier>();
            _team1Bees = state.GetEntityQuery(builder);
            _team1Bees.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 0});
            _team2Bees = state.GetEntityQuery(builder);
            _team2Bees.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});

            _random = Random.CreateFromIndex(4000);
            state.RequireForUpdate<BeeConfig>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            
            var beeConfig = SystemAPI.GetSingleton<BeeConfig>();

            var team1Bees = _team1Bees.ToComponentDataArray<Bee>(Allocator.TempJob);
            var team2Bees = _team1Bees.ToComponentDataArray<Bee>(Allocator.TempJob);

            var team1Job = new SwarmJob
            {
                BeeComponents = team1Bees,
                Dt = dt,
                Seed = _random.NextUInt(),
                TeamAttraction = beeConfig.Team1.TeamAttraction
            };

            var team2Job = new SwarmJob
            {
                BeeComponents = team2Bees,
                Dt = dt,
                Seed = _random.NextUInt(),
                TeamAttraction = beeConfig.Team2.TeamAttraction
            };

            var team1Handle = team1Job.ScheduleParallel(_team1Bees, state.Dependency);
            var team2Handle =team2Job.ScheduleParallel(_team2Bees, team1Handle);
            
            team2Handle.Complete();

            team1Bees.Dispose();
            team2Bees.Dispose();
        }
    }
}