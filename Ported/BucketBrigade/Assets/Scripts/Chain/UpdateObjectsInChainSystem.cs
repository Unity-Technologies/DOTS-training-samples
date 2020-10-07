using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public class UpdateObjectsInChainSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private List<SharedChainComponent> m_Chains;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_Chains = new List<SharedChainComponent>(2); // TODO capacity should be equal to number of chains in settings
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        EntityManager.GetAllUniqueSharedComponentData(m_Chains);
        for (int chainIndex = 0; chainIndex < m_Chains.Count; ++chainIndex)
        {
            var chain = m_Chains[chainIndex];
            float2 startPosition = new float2();
            float2 endPosition = new float2();
            int chainLength = 0;

            Entities
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        in ChainStart start, in ChainEnd end, in ChainLength length, in ChainID id) =>
                    {
                        if (chain.chainID == id.Value)
                        {
                            startPosition = start.Value;
                            endPosition = end.Value;
                            chainLength = length.Value;
                        }
                    })
                .Run();

            if (chainLength > 0)
            {
                Entities
                    .WithSharedComponentFilter(chain)
                    .WithName("SetChainTargets")
                    .WithNone<Target>()
                    .ForEach(
                        (Entity entity, int entityInQueryIndex,
                            ref Pos pos, ref Speed speed,
                            in CurrentBotCommand currentCommand,
                            in ChainPosition position) =>
                        {
                            var targetPosition =
                                GetChainPosition(position.Value, chainLength, startPosition, endPosition);
                            ecb.AddComponent(entityInQueryIndex, entity, new Target() {Value = targetPosition});
                        })
                    .ScheduleParallel();

                Entities
                    .WithSharedComponentFilter(chain)
                    .WithName("UpdateObjectsInChain")
                    .ForEach(
                        (Entity entity, int entityInQueryIndex,
                            ref Pos pos, ref Speed speed, ref Target target,
                            in CurrentBotCommand currentCommand,
                            in ChainPosition position) =>
                        {
                            var targetPosition =
                                GetChainPosition(position.Value, chainLength, startPosition, endPosition);
                            target.Value = targetPosition;
                            if (currentCommand.Command != Command.Move)
                            {
                                pos.Value = targetPosition;
                            }
                        })
                    .ScheduleParallel();
            }
        }

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    static float2 GetChainPosition(int _index, int _chainLength, float2 _startPos, float2 _endPos)
    {
        // TODO check validity of this comment
        // adds two to pad between the SCOOPER AND THROWER
        float progress = (float)_index / _chainLength;
        float curveOffset = math.sin(progress * math.PI) * 1f;

        // get float2 data
        float2 heading = new float2(_startPos.x, _startPos.y) - new float2(_endPos.x, _endPos.y);
        float distance = math.length(heading);
        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);

        return math.lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + (new float2(perpendicular.x, perpendicular.y) * curveOffset);
    }
}