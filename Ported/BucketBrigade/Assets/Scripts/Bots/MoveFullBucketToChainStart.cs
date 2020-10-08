using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateBefore(typeof(MoveTowardsTargetSystem))]
public class MoveFullBucketToChainStart : SystemBase
{
    private EntityQuery waterQuery;
    private List<SharedChainComponent> m_Chains;

    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(ComponentType.ReadOnly<ChainStart>(), ComponentType.ReadOnly<ChainID>());
        
        m_Chains = new List<SharedChainComponent>(2);
    }
    
    protected override void OnUpdate()
    {
        m_Chains.Clear();
        EntityManager.GetAllUniqueSharedComponentData(m_Chains);
        NativeArray<SharedChainComponent> tempChains = new NativeArray<SharedChainComponent>(m_Chains.ToArray(), Allocator.TempJob);
            
        Entities
            .WithName("MoveToChainStart")
            .WithNone<ChainPosition>()
            .WithAll<BotTag>()
            .WithDisposeOnCompletion(tempChains)
            .ForEach((ref Target target, ref DroppingOffBucket droppingOffBucket,
                in Pos pos, in HasBucket hasBucket, in FillingBucket fillingBucket) =>
            {
                if (!hasBucket.PickedUp || !fillingBucket.Full)
                    return;
                
                target.Position = GetNearestChainStart(pos.Value, tempChains, out SharedChainComponent chain);
                target.Entity = Entity.Null;
                droppingOffBucket.DroppingOff = true;
                droppingOffBucket.chain = chain;
            }).ScheduleParallel();
    }

    static float2 GetNearestChainStart(in float2 pos, in NativeArray<SharedChainComponent> chains, out SharedChainComponent chain)
    {
        float nearestDist = float.MaxValue;
        float2 nearestPos = pos;
        chain = new SharedChainComponent();

        for (int i = 0; i < chains.Length; i++)
        {
            float dist = math.distancesq(chains[i].start, pos);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestPos = chains[i].start;
                chain = chains[i];
            }
        }

        return nearestPos;
    }
}