using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class MoveTowardsTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithName("MoveTowardsTarget")
            .ForEach((ref Pos pos, ref Target target,
                in Speed speed) =>
            {
                float speedThisFrame = speed.Value * deltaTime;
                float2 offset = target.Position - pos.Value;
                target.ReachedTarget = math.lengthsq(offset) <= speedThisFrame * speedThisFrame;
                
                if (target.ReachedTarget)
                {
                    pos.Value = target.Position;
                }
                else
                {
                    pos.Value += math.normalize(offset) * speedThisFrame;
                }
            }).ScheduleParallel();
    }
}

public class MoveFullBucketToChainStart : SystemBase
{
    private EntityQuery waterQuery;

    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(ComponentType.ReadOnly<ChainStart>(), ComponentType.ReadOnly<ChainID>());
    }
    
    protected override void OnUpdate()
    {
        NativeArray<ChainStart> chainStarts = waterQuery.ToComponentDataArray<ChainStart>(Allocator.Temp);
        NativeArray<ChainID> chainIDs = waterQuery.ToComponentDataArray<ChainID>(Allocator.Temp);
        
        // This is going to be slow because of how it's a Run!!!
        Entities
            .WithName("MoveToChainStart")
            .WithNone<ChainPosition>()
            .WithAll<BotTag>()
            .WithoutBurst()
            //.WithDisposeOnCompletion(chainStarts)
            //.WithDisposeOnCompletion(chainIDs)
            .ForEach((ref Target target, in HasBucket hasBucket, in FillingBucket fillingBucket, in SharedChainComponent targetChain) =>
            {
                if (!hasBucket.PickedUp || !fillingBucket.Full)
                    return;
                
                // Look for matching Chain ID.
                for (int i = 0; i < chainIDs.Length; i++)
                {
                    if (chainIDs[i].Value == targetChain.chainID)
                    {
                        target.Position = chainStarts[i].Value;
                        target.Entity = Entity.Null;
                        
                        break;
                    }
                }
            }).Run();
    }
}