using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [WithAll(typeof(Bee))]
    [BurstCompile]
    partial struct SafeZoneJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<Bee> Bees;
        public EntityQuery BeeQuery;
        public Random Random;
        public float Dt;

        public float SquaredRadius;

        void Execute(Entity entity, Bee currentBee)
        {
            var teamAttraction = SystemAPI.GetSingleton<BeeConfig>().Team1.TeamAttraction;
            
            var allBees = BeeQuery.ToEntityArray(Allocator.Temp);
            var targetEntity = allBees[Random.NextInt(allBees.Length)];
            Bees.TryGetComponent(targetEntity, out var randomBee);
            
            float3 delta = randomBee.Position - currentBee.Position;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f)
            {
                currentBee.Velocity += delta * (teamAttraction * Dt / dist);
            }
        }
    }
    
    public partial class BeeSwarmingSystem : SystemBase
    {
        private EntityQuery _beeQuery;
        private ComponentLookup<Bee> _bees;
        private Random _random;

        protected override void OnCreate()
        {
            _random = Random.CreateFromIndex(4000);
            _beeQuery = GetEntityQuery(typeof(Bee));
            _bees = GetComponentLookup<Bee>();
        }

        protected override void OnUpdate()
        {
            var dt = SystemAPI.Time.DeltaTime;
            // var allBees = _beeQuery.ToEntityArray(Allocator.Temp);
            // var teamAttraction = SystemAPI.GetSingleton<BeeConfig>().Team1.TeamAttraction;
            //
            

            // Bee GetRandom()
            // {
            //     var targetEntity = allBees[_random.NextInt(allBees.Length)];
            //     _bees.TryGetComponent(targetEntity, out var randomBee);
            //     return randomBee;
            // }
            //
            // Entities
            //     .WithAll<Bee>()
            //     .ForEach((ref Entity entity, ref Bee currentBee) =>
            //     {
            //         var randomBee = GetRandom();
            //         
            //         float3 delta = randomBee.Position - currentBee.Position;
            //         float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            //         if (dist > 0f)
            //         {
            //             currentBee.Velocity += delta * (teamAttraction * dt / dist);
            //         }
            //     })
            //     .WithoutBurst()
            //     .ScheduleParallel(Dependency);

            var job = new SafeZoneJob()
            {
                BeeQuery = _beeQuery,
                Bees = _bees,
                Dt = dt
            };

            job.ScheduleParallel();
        }
    }
}