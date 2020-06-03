using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
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

            // Nearest water TBC....
            var nearestWater = new float3(rand.NextFloat(gridSize.x), 0, rand.NextFloat(gridSize.y));

            // Nearest fire TBC....
            var nearestFire = new float3(rand.NextFloat(gridSize.x), 0, rand.NextFloat(gridSize.y));

            for (int i = 0; i < config.NumberOfChains; ++i)
            {
                var chain = EntityManager.CreateEntity(m_ChainArchetype);
                SetChainStartEnd(chain, nearestWater, nearestFire);

                // Scooper
                var scooper = CreateAgent(ref rand, prefabs.ScooperPrefab, gridSize);
                SetTargetPosition(scooper, nearestWater);

                // Thrower
                var thrower = CreateAgent(ref rand, prefabs.ThrowerPrefab, gridSize);
                SetTargetPosition(thrower, nearestFire);
                
                // Forward Chain
                CreateChain(ref rand, chain, config, prefabs.PasserForwardPrefab, gridSize);
                
                // Backwards Chain
                CreateChain(ref rand, chain, config, prefabs.PasserBackPrefab, gridSize, -1f);
            }
        }

        private void SetTargetPosition(Entity agent, float3 position)
        {
            var targetPosition = EntityManager.GetComponentData<TargetPosition>(agent);
            targetPosition.Target = position;
            EntityManager.SetComponentData(agent, targetPosition);
        }

        private void SetChainStartEnd(Entity chain, float3 nearestWater, float3 nearestFire)
        {
            var chainComponent = EntityManager.GetComponentData<Chain>(chain);
            chainComponent.ChainStartPosition = nearestWater;
            chainComponent.ChainEndPosition = nearestFire;
            EntityManager.SetComponentData(chain, chainComponent);
        }

        private void CreateChain(ref Random rand, in Entity chain, in BucketBrigadeConfig config,
            in Entity prefabEntity, in float2 gridSize, float forward = 1f)
        {
            var chainComponent = EntityManager.GetComponentData<Chain>(chain);
            var direction = math.normalize(chainComponent.ChainEndPosition - chainComponent.ChainStartPosition);
            var perpendicular = new float3(direction.z, 0f, -direction.x);
            
            // 1. Randomly distribute a Scooper, a Thrower and N Passers.
            
            for (int i = 0; i < config.NumberOfPassersInOneDirectionPerChain; ++i)
            {
                var agent = CreateAgent(ref rand, prefabEntity, gridSize);
                var agentComponent = EntityManager.GetComponentData<Agent>(agent);
                agentComponent.ChainT = i / (float) (config.NumberOfPassersInOneDirectionPerChain - 1);
                agentComponent.MyChain = chain;
                EntityManager.SetComponentData(agent, agentComponent);

                // TODO: factor this out of ChainInit.
                var offset = math.sin(agentComponent.ChainT * math.PI);
                var target = math.lerp(chainComponent.ChainStartPosition, chainComponent.ChainEndPosition,
                    agentComponent.ChainT);
                target += perpendicular * (offset * forward);
                SetTargetPosition(agent,target);
            }
            
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