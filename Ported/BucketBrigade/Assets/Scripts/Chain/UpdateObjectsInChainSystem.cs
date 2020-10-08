using System.Collections.Generic;
using System.Diagnostics;
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
        var bucketsArray = EntityManager.CreateEntity();
        EntityManager.AddBuffer<BucketInChain>(bucketsArray);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var bucketsArrayEntity = GetSingletonEntity<BucketInChain>();
        var bucketsArray = EntityManager.GetBuffer<BucketInChain>(bucketsArrayEntity);

        EntityManager.GetAllUniqueSharedComponentData(m_Chains);
        for (int chainIndex = 0; chainIndex < m_Chains.Count; ++chainIndex)
        {
            var chain = m_Chains[chainIndex];
            var id = m_Chains[chainIndex].chainID;
            var startPosition = m_Chains[chainIndex].start;
            var endPosition = m_Chains[chainIndex].end;
            var chainLength = m_Chains[chainIndex].length;

            Entities
                .WithSharedComponentFilter(chain)
                .WithName("SetChainTargets")
                .WithNone<Target>()
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        ref Pos pos, ref Speed speed,
                        in ChainPosition position) =>
                    {
                        var targetPosition =
                            GetChainPosition(position.Value, 0f, chainLength, startPosition, endPosition);
                        ecb.AddComponent(entityInQueryIndex, entity, new Target() {Position = targetPosition});
                    })
                .ScheduleParallel();

            Entities
                .WithSharedComponentFilter(chain)
                .WithName("UpdateObjectsInChain")
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        ref Pos pos, ref Speed speed, ref Target target,
                        in ChainPosition position) =>
                    {
                        if (target.ReachedTarget)
                        {
                            if (position.Value == 0 && bucketsArray.Length == 0)
                            {
                                // here we should add chain specific components to the bucket, so it would be processed with the chain, and fill it
                                bucketsArray.Add(new BucketInChain() { chainID = id, bucketPos = 0, bucketShift = 0 });
                            }
                            float shift = 0f;
                            for (int i = 0; i < bucketsArray.Length; ++i)
                            {
                                if (bucketsArray[i].chainID == id)
                                {
                                    if (position.Value == bucketsArray[i].bucketPos)
                                    {
                                        shift = bucketsArray[i].bucketShift;
                                        if (shift < 1f)
                                        {
                                            bucketsArray[i] = new BucketInChain()
                                            {
                                                chainID = bucketsArray[i].chainID,
                                                bucketPos = bucketsArray[i].bucketPos,
                                                bucketShift = bucketsArray[i].bucketShift + 0.001f
                                            };
                                        }
                                    }
                                    else if (position.Value == (bucketsArray[i].bucketPos + 1) % (chainLength * 2))
                                    {
                                        if (bucketsArray[i].bucketShift >= 1f)
                                        {
                                            bucketsArray[i] = new BucketInChain()
                                            {
                                                chainID = bucketsArray[i].chainID,
                                                bucketPos = (bucketsArray[i].bucketPos + 1) % (chainLength * 2),
                                                bucketShift = 0f
                                            };
                                            if (bucketsArray[i].bucketPos + 1 == chainLength)
                                            {
                                                // here we're processing bucket we should throw bucket to the water and mark it empty
                                            }
                                        }
                                    }
                                    else if (position.Value == bucketsArray[i].bucketPos - 1)
                                        shift = 1f - bucketsArray[i].bucketShift;
                                }
                            }
                            var targetPosition = GetChainPosition(position.Value, shift, chainLength, startPosition, endPosition);
                            target.Position = targetPosition;
                            pos.Value = targetPosition;
                        }
                        else
                        {
                            var targetPosition = GetChainPosition(position.Value, 0f, chainLength, startPosition, endPosition);
                            target.Position = targetPosition;
                        }
                    })
                .Schedule/*Parallel*/();
        }

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    static float2 GetChainPosition(int _index, float _shift, int _chainLength, float2 _startPos, float2 _endPos)
    {
        var dir = 1.0f;
        float shiftedIndex = _index + _shift;
        if (shiftedIndex > _chainLength)
        {
            dir = -1.0f;
            shiftedIndex = 2 * _chainLength - shiftedIndex;
        }
        float progress = shiftedIndex / _chainLength;
        float curveOffset = math.sin(progress * math.PI) * 1f;

        // get float2 data
        float2 heading = (new float2(_startPos.x, _startPos.y) - new float2(_endPos.x, _endPos.y)) * dir;
        float distance = math.length(heading);
        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);

        return math.lerp(_startPos, _endPos, progress) + (new float2(perpendicular.x, perpendicular.y) * curveOffset);
    }
}