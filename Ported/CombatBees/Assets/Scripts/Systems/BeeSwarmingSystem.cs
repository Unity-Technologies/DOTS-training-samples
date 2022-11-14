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
            var allBees = _beeQuery.ToEntityArray(Allocator.Temp);
            var teamAttraction = SystemAPI.GetSingleton<BeeConfig>().Team1.TeamAttraction;

            Entities
                .WithAll<Bee>()
                .ForEach((Entity entity, Bee currentBee) =>
                {
                    var targetEntity = allBees[_random.NextInt(allBees.Length)];
                    _bees.TryGetComponent(targetEntity, out var randomBee);
                    
                    float3 delta = randomBee.Position - currentBee.Position;
                    float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    if (dist > 0f)
                    {
                        currentBee.Velocity += delta * (teamAttraction * dt / dist);
                    }
                })
                .ScheduleParallel(Dependency);
        }
    }
}