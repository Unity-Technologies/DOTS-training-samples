using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

        void Execute([EntityInQueryIndex] int index, Entity entity, ref Bee currentBee)
        {
            var newRandom = Random.CreateFromIndex((uint)index + Seed);
            var randomBee = BeeComponents[newRandom.NextInt(BeeComponents.Length)];

            float3 delta = randomBee.Position - currentBee.Position;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f)
            {
                currentBee.Velocity += delta * (TeamAttraction * Dt / dist);
            }
        }
    }

    public partial struct BeeSwarmingSystem : ISystem
    {
        private EntityQuery _beeQuery;
        private ComponentLookup<Bee> _bees;
        private Random _random;
        private uint _randomSeed;


        public void OnCreate(ref SystemState state)
        {
            _random = Random.CreateFromIndex(4000);
            _beeQuery = state.GetEntityQuery(typeof(Bee));
            _bees = state.GetComponentLookup<Bee>();
            state.RequireForUpdate<BeeConfig>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            _bees.Update(ref state);
            
            var teamAttraction = SystemAPI.GetSingleton<BeeConfig>().Team1.TeamAttraction;
            //var bees = _beeQuery.ToEntityArray(Allocator.TempJob);
            var beeComponents = _beeQuery.ToComponentDataArray<Bee>(Allocator.TempJob);

            var job = new SwarmJob()
            {
                //Entities = bees,
                BeeComponents = beeComponents,
                Dt = dt,
                Seed = _random.NextUInt(),
                TeamAttraction = teamAttraction
            };

            job.Schedule();

            beeComponents.Dispose(state.Dependency);
        }
    }
}