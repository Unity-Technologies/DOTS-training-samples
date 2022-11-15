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
        [ReadOnly] public NativeArray<Physical> PhysicalComponents;
        public float TeamAttraction;
        public float Dt;
        public uint Seed;

        void Execute([EntityInQueryIndex] int index, ref Bee currentBee, ref Physical currentPhysical)
        {
            var newRandom = Random.CreateFromIndex((uint)index + Seed);
            var randomBee = PhysicalComponents[newRandom.NextInt(PhysicalComponents.Length)];

            float3 delta = randomBee.Position - currentPhysical.Position;
            float dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f)
            {
                var velocity = currentPhysical.Velocity;
                velocity += delta * (TeamAttraction * Dt / dist);
                currentPhysical.Velocity = velocity;
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
            _team1Bees = SystemAPI.QueryBuilder().WithAll<Bee, TeamIdentifier, Physical>().Build();
            _team1Bees.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 0});
            _team2Bees = SystemAPI.QueryBuilder().WithAll<Bee, TeamIdentifier, Physical>().Build();
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
            var team1Bees = _team1Bees.ToComponentDataArray<Physical>(Allocator.TempJob);
            var team2Bees = _team1Bees.ToComponentDataArray<Physical>(Allocator.TempJob);

            var team1Job = new SwarmJob
            {
                PhysicalComponents = team1Bees,
                Dt = dt,
                Seed = _random.NextUInt(),
                TeamAttraction = beeConfig.Team1.TeamAttraction
            };

            var team2Job = new SwarmJob
            {
                PhysicalComponents = team2Bees,
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