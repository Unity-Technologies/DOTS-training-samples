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
        float deltaTime = Time.DeltaTime;
        
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var bucketsArrayEntity = GetSingletonEntity<BucketInChain>();
        var bucketsArray = EntityManager.GetBuffer<BucketInChain>(bucketsArrayEntity);

        m_Chains.Clear();
        EntityManager.GetAllUniqueSharedComponentData(m_Chains);
        for (int chainIndex = 0; chainIndex < m_Chains.Count; ++chainIndex)
        {
            var chain = m_Chains[chainIndex];
            var id = m_Chains[chainIndex].chainID;
            var startPosition = m_Chains[chainIndex].start;
            var endPosition = m_Chains[chainIndex].end;
            var chainLength = m_Chains[chainIndex].length;
            var stepLength = math.length(endPosition - startPosition) / chainLength;

            Entities
                .WithSharedComponentFilter(chain)
                .WithName("SetChainTargets")
                .WithNone<Target>()
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        in ChainPosition position) =>
                    {
                        var targetPosition = GetChainPosition(position.Position, position.Shift, chainLength, startPosition, endPosition);
                        ecb.AddComponent(entityInQueryIndex, entity, new Target() {Position = targetPosition});
                    })
                .ScheduleParallel();

            Entities
                .WithSharedComponentFilter(chain)
                .WithName("UpdateObjectsInChain")
                .WithReadOnly(bucketsArray)
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        ref Pos pos, ref Target target, ref ChainPosition position,
                        in Speed speed, in ChainObjectType type) =>
                    {
                        if (target.ReachedTarget)
                        {
                            int bucketIndex = -1;
                            var bucketToGrabIndex = -1;
                            for (int i = 0; i < bucketsArray.Length; ++i)
                            {
                                if (bucketsArray[i].chainID == id)
                                {
                                    if (position.Position == bucketsArray[i].bucketPos)
                                    {
                                        bucketIndex = i;
                                    }
                                    if (bucketsArray[i].bucketShift >= 0.99f &&
                                        NextPos(bucketsArray[i].bucketPos, chainLength) == position.Position &&
                                        position.Shift <= 0.01f)
                                    {
                                        bucketToGrabIndex = i;
                                    }
                                }
                            }

                            if (bucketIndex >= 0 && position.Shift < 1.0f)
                            {
                                var shift = speed.Value * deltaTime / stepLength;
                                position.Shift += shift;
                                var updateBucket = ecb.CreateEntity(entityInQueryIndex);
                                ecb.AddComponent(entityInQueryIndex, updateBucket, new UpdateBucketInChain()
                                {
                                    index = bucketIndex,
                                    bucketPos = bucketsArray[bucketIndex].bucketPos,
                                    bucketShift = bucketsArray[bucketIndex].bucketShift + shift
                                });
                            }
                            else if (type.Value == ObjectType.Bucket && bucketIndex >= 0 && position.Shift >= 0.99f)
                            {
                                var newPos = NextPos(bucketsArray[bucketIndex].bucketPos, chainLength);
                                if (type.Value == ObjectType.Bucket)
                                {
                                    position.Position = newPos;
                                    position.Shift = 0f;
                                    if (newPos == chainLength)
                                    {
                                        ecb.AddComponent<ThrowTag>(entityInQueryIndex, entity);
                                    }
                                    else if (newPos == 0)
                                    {
                                        ecb.AddComponent<FillTag>(entityInQueryIndex, entity);
                                    }
                                }
                            }
                            else if (bucketIndex == -1 && position.Shift >= 0f)
                            {
                                var shift = speed.Value * deltaTime / stepLength;
                                position.Shift -= shift;
                            }
                            if (bucketToGrabIndex >= 0)
                            {
                                var newPos = NextPos(bucketsArray[bucketToGrabIndex].bucketPos, chainLength);
                                var updateBucket = ecb.CreateEntity(entityInQueryIndex);
                                ecb.AddComponent(entityInQueryIndex, updateBucket, new UpdateBucketInChain()
                                {
                                    index = bucketToGrabIndex,
                                    bucketPos = newPos,
                                    bucketShift = 0f
                                });
                            }
                            var targetPosition = GetChainPosition(position.Position, position.Shift, chainLength, startPosition, endPosition);
                            target.Position = targetPosition;
                            pos.Value = targetPosition;
                        }
                        else
                        {
                            var targetPosition = GetChainPosition(position.Position, position.Shift, chainLength, startPosition, endPosition);
                            target.Position = targetPosition;
                        }
                    })
                .ScheduleParallel();
        }

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    static int NextPos(int pos, int length)
    {
        return length > 0 ? (pos + 1) % (length * 2) : 0;
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