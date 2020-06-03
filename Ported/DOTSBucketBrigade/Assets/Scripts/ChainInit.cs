using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    public class ChainInit : SystemBase
    {
        private EntityQuery m_ChainQuery;
        private EntityArchetype m_ChainArchetype;

        protected override void OnCreate()
        {
            m_ChainQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new []{ ComponentType.ReadOnly<Chain>() }
                }
            );

            m_ChainArchetype = EntityManager.CreateArchetype(typeof(Chain));
        }

        protected override void OnUpdate()
        {
            if (m_ChainQuery.CalculateChunkCount() > 0)
                return;

            var config = GetSingleton<BucketBrigadeConfig>();
            var prefabs = GetSingleton<SpawnerConfig>();

            var rand = new Unity.Mathematics.Random();
            rand.InitState();
            
            float2 gridSize = (float2)config.GridDimensions * config.CellSize;

            for (int i = 0; i < config.NumberOfChains; ++i)
            {
                var chain = EntityManager.CreateEntity(m_ChainArchetype);
                
                // Scooper
                var scooper = CreateAgent(ref rand, prefabs.ScooperPrefab, gridSize);
                
                // Forward Chain
                CreateChain(ref rand, config, prefabs.PasserForwardPrefab, gridSize);
                
                // Backwards Chain
                CreateChain(ref rand, config, prefabs.PasserBackPrefab, gridSize);
                
                // Thrower
                var thrower = CreateAgent(ref rand, prefabs.ThrowerPrefab, gridSize);
            }
        }

        private void CreateChain(ref Random rand, in BucketBrigadeConfig config, in Entity prefabEntity, in float2 gridSize)
        {
            // 1. Randomly distribute a Scooper, a Thrower and N Passers.
            
            for (int i = 0; i < config.NumberOfPassersInOneDirectionPerChain; ++i)
            {
                CreateAgent(ref rand, prefabEntity, gridSize);
            }

            // 2. The entity with a Chain component

            // 3. Scooper at the start.

            // 4. Thrower at the end.

            // 5. Line of passers from the start to the end

            // 6. Line of passers from the end to the start

            // - What happens with the Scooper with respect to finding the 'start' of the chain, to start passing?
            // - What happens with the Thrower with respect to finding the 'end' of the chain, to pass back to?
        }

        private Entity CreateAgent(ref Random rand, Entity prefabEntity, float2 gridSize)
        {
            var agent = EntityManager.Instantiate(prefabEntity);
            var agentPos = GetComponent<Translation>(agent);
            agentPos.Value = new float3(rand.NextFloat(gridSize.x), 0, rand.NextFloat(gridSize.y));
            SetComponent(agent, agentPos);
            return agent;
        }
    }
}