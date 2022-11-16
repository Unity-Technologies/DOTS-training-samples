﻿using System;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [WithAll(typeof(Bee))]
    [BurstCompile]
    partial struct SwarmJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Physical> PhysicalComponents;
        public float TeamAttraction;
        public float Dt;
        public uint Seed;

        void Execute([EntityInQueryIndex] int index, ref Physical currentPhysical)
        {
            var newRandom = Random.CreateFromIndex((uint)index + Seed);
            var randomBee1 = PhysicalComponents[newRandom.NextInt(PhysicalComponents.Length)];
            var randomBee2 = PhysicalComponents[newRandom.NextInt(PhysicalComponents.Length)];
            UpdateVelocity(ref currentPhysical, ref randomBee1, 1, 1f);
            UpdateVelocity(ref currentPhysical, ref randomBee2, -1, 0.8f);
        }

        void UpdateVelocity(ref Physical currentPhysical, ref Physical randomPhysical, int sign, float power)
        {
            float3 delta = randomPhysical.Position - currentPhysical.Position;
            float dist = math.length(delta);
            if (dist > 0f)
            {
                var velocity = currentPhysical.Velocity;
                velocity += delta * (TeamAttraction * power * Dt / dist) * sign;
                
                currentPhysical.Velocity = velocity;
            }
        }
    }
    
    [BurstCompile]
    public partial struct BeeSwarmingSystem : ISystem
    {
        private Random _random;
        private uint _randomSeed;
        private EntityQuery beeQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Bee>();

            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<Bee, TeamIdentifier, Physical>().WithNone<Dead>();
            beeQuery = state.GetEntityQuery(builder);
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});
            
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
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});
            var team1Bees = beeQuery.ToComponentDataArray<Physical>(Allocator.TempJob);
            
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 2});
            var team2Bees = beeQuery.ToComponentDataArray<Physical>(Allocator.TempJob);

            var seed1 = _random.NextUInt();
            var seed2 = _random.NextUInt();
            
            var team1Job = new SwarmJob
            {
                PhysicalComponents = team1Bees,
                Dt = dt,
                Seed = seed1,
                TeamAttraction = beeConfig.Team1.TeamAttraction
            };

            var team2Job = new SwarmJob
            {
                PhysicalComponents = team2Bees,
                Dt = dt,
                Seed = seed2,
                TeamAttraction = beeConfig.Team2.TeamAttraction
            };

            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 2});
            var team2Handle = team2Job.ScheduleParallel(beeQuery, state.Dependency);
            
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});
            var team1Handle = team1Job.ScheduleParallel(beeQuery, state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(team1Handle, team2Handle);

            team1Bees.Dispose(state.Dependency);
            team2Bees.Dispose(state.Dependency);
        }
    }
}