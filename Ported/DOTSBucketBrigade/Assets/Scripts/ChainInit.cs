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
                var scooper = CreateAgent(ref rand, prefabs.ScooperPrefab, gridSize, chain, 0f, 
                    1f);
                SetTargetPosition(scooper, nearestWater);

                // Thrower
                var thrower = CreateAgent(ref rand, prefabs.ThrowerPrefab, gridSize, chain, 1f, -1f);
                SetTargetPosition(thrower, nearestFire);
                
                // Forward Chain
                // Start (Water) -> End (Fire)
                // Scooper -> Head .. Tail -> Thrower
                CreateHalfOfChain(ref rand, chain, config, prefabs.PasserForwardPrefab, gridSize, out var forwardHead, out var forwardTail);
                SetNextInChain(scooper, forwardHead);
                SetNextInChain(forwardTail, thrower);
                
                // Backwards Chain
                // Start (Fire) -> End (Water)
                // Thrower -> Head .. Tail -> Scooper
                CreateHalfOfChain(ref rand, chain, config, prefabs.PasserBackPrefab, gridSize, out var backwardsHead, out var backwardsTail, -1f);
                SetNextInChain(thrower, backwardsHead);
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

        private void CreateHalfOfChain(ref Random rand, in Entity chain, in BucketBrigadeConfig config,
            in Entity prefabEntity, in float2 gridSize, out Entity head, out Entity tail, float forward = 1f)
        {
            var chainComponent = EntityManager.GetComponentData<Chain>(chain);
            var direction = math.normalize(chainComponent.ChainEndPosition - chainComponent.ChainStartPosition) * forward;
            var perpendicular = new float3(direction.z, 0f, -direction.x);
            
            head = Entity.Null;
            tail = Entity.Null;
            Entity nextInChain = Entity.Null;
            
            // 1. Randomly distribute a Scooper, a Thrower and N Passers.
            
            for (int i = config.NumberOfPassersInOneDirectionPerChain - 1; i >= 0; --i)
            {
                var agent = CreateAgent(ref rand, prefabEntity, gridSize, chain, i / (float) (config.NumberOfPassersInOneDirectionPerChain - 1), forward);
                if (i == config.NumberOfPassersInOneDirectionPerChain - 1)
                    tail = agent;
                if (i == 0)
                    head = agent;
                var agentComponent = EntityManager.GetComponentData<Agent>(agent);
                float3 target;
                if (forward > 0)
                {
                    target = CalculateChainPosition(agentComponent, perpendicular, chainComponent.ChainStartPosition, chainComponent.ChainEndPosition);
                }
                else
                {
                    target = CalculateChainPosition(agentComponent, perpendicular, chainComponent.ChainEndPosition, chainComponent.ChainStartPosition); 
                }
                SetTargetPosition(agent, target);
                SetNextInChain(agent, nextInChain);
                nextInChain = agent;
            }
        }

        private void SetNextInChain(Entity agent, Entity prev)
        {
            EntityManager.SetComponentData(agent, new NextInChain { Next = prev });
        }

        public static float3 CalculateChainPosition(in Agent agentComponent, in float3 perpendicular, in float3 start, in float3 end)
        {
            var offset = math.sin(agentComponent.ChainT * math.PI);
            var target = math.lerp(start, end,
                agentComponent.ChainT);
            target += perpendicular * offset;
            return target;
        }

        private Entity CreateAgent(ref Random rand, Entity prefabEntity, float2 gridSize, Entity chain, float chainT, float forward)
        {
            var agent = EntityManager.Instantiate(prefabEntity);
            var agentPos = GetComponent<Translation>(agent);
            agentPos.Value = new float3(rand.NextFloat(gridSize.x), 0, rand.NextFloat(gridSize.y));
            SetComponent(agent, agentPos);
            
            var agentComponent = EntityManager.GetComponentData<Agent>(agent);
            agentComponent.ChainT = chainT;
            agentComponent.MyChain = chain;
            agentComponent.Forward = forward;
            EntityManager.SetComponentData(agent, agentComponent);
            return agent;
        }
    }
}