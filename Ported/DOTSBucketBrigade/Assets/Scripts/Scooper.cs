using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Movement))]
    public class Scooper : SystemBase
    {
        private EntityQuery m_WaterSourceQuery;
        private EntityQuery m_BucketQuery;
        private EndSimulationEntityCommandBufferSystem m_Barrier;

        protected override void OnCreate()
        {
            // TODO: this is going to find water sources that are not empty.
            m_WaterSourceQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new []{ComponentType.ReadOnly<WaterSource>() }
                });
            
            m_BucketQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new []{ComponentType.ReadOnly<AvailableBucketTag>() }
                });

            m_Barrier =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var config = GetSingleton<BucketBrigadeConfig>();
            var ecb = m_Barrier.CreateCommandBuffer().ToConcurrent();
            
            // TODO: this does not need to be queried every frame.
            var waterEntities = m_WaterSourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var fetchWaterEntitiesJob);
            var bucketEntities = m_BucketQuery.ToEntityArrayAsync(Allocator.TempJob, out var fetchBucketEntitiesJob);
            var bucketAssignments = new NativeArray<byte>(bucketEntities.Length, Allocator.TempJob);

            var combinedFetchJob = JobHandle.CombineDependencies(Dependency, fetchWaterEntitiesJob, fetchBucketEntitiesJob);
            
            var translationComponent = GetComponentDataFromEntity<LocalToWorld>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            var waterLevelComponent = GetComponentDataFromEntity<WaterLevel>();
            var targetBucketComponent = GetComponentDataFromEntity<TargetBucket>();
            var bucketColorComponent = GetComponentDataFromEntity<BucketColor>();

            Dependency = Entities.WithNone<BucketChangeRequest>().ForEach((Entity entity, int entityInQueryIndex, ref ScooperState state, ref TargetPosition targetPosition, ref TargetWaterSource targetWaterSource, in NextInChain nextInChain, in Translation position, in Agent agent)
                =>
            {
                // TODO: Commonality between
                // - Start Moving To Target & Wait Until Target In Range...
                // - Find Resource
                var myChain = chainComponent[agent.MyChain];
                var targetBucket = targetBucketComponent[entity];
                switch (state.State)
                {
                    case EScooperState.FindWater:
                        // Find closest water to my position
                        var nearestWater = FindNearestWaterWithCapacity(translationComponent, waterLevelComponent, waterEntities, position, config.BucketCapacity);
                        
                        if (nearestWater == Entity.Null)
                            break;
                        
                        var nearestWaterPosition = translationComponent[nearestWater];

                        myChain.ChainStartPosition = nearestWaterPosition.Position;
                        chainComponent[agent.MyChain] = myChain;

                        targetWaterSource.Target = nearestWater;
                        if (targetBucket.Target != Entity.Null)
                        {
                            state.State = EScooperState.StartWalkingToWater;
                        }
                        else
                        {
                            state.State = EScooperState.FindBucket;
                        }
                        break;
                    
                    case EScooperState.StartWalkingToWater:
                        var walkToWaterPosition = translationComponent[targetWaterSource.Target];
                        targetPosition.Target = walkToWaterPosition.Position;
                        state.State = EScooperState.WaitUntilWaterInRange;
                        break;
                    
                    case EScooperState.WaitUntilWaterInRange:
                        var targetWaterPosition = translationComponent[targetWaterSource.Target];
                        var waterDistSq = math.distancesq(targetWaterPosition.Position.xz, position.Value.xz);

                        if (waterDistSq < config.MovementTargetReachedThreshold)
                        {
                            state.State = EScooperState.FillBucket;
                        }
                        break;
                    
                    case EScooperState.FindBucket:
                        var nearestBucket = FindNearestBucket(translationComponent, bucketEntities, bucketAssignments, position, out var foundIndex);
                        if (nearestBucket == Entity.Null)
                            break;

                        bucketAssignments[foundIndex] = 1;
                        
                        var nearestBucketPosition = translationComponent[nearestBucket];
                        targetBucket.Target = nearestBucket;
                        targetBucketComponent[entity] = targetBucket;
                        targetPosition.Target = nearestBucketPosition.Position;
                        state.State = EScooperState.StartWalkingToBucket;
                        break;                    
                    
                    case EScooperState.StartWalkingToBucket:
                        var walkToBucketPosition = translationComponent[targetBucket.Target];
                        targetPosition.Target = walkToBucketPosition.Position;
                        state.State = EScooperState.WaitUntilBucketInRange;
                        break;
                    
                    case EScooperState.WaitUntilBucketInRange:
                        var targetBucketPosition = translationComponent[targetBucket.Target];
                        var bucketDistSq = math.distancesq(targetBucketPosition.Position.xz, position.Value.xz);

                        if (bucketDistSq < config.MovementTargetReachedThreshold)
                        {
                            ecb.RemoveComponent<AvailableBucketTag>(entityInQueryIndex, targetBucket.Target);
                            state.State = EScooperState.StartWalkingToWater;
                        }
                        break;
                    
                    case EScooperState.FillBucket:
                        // TODO: concurrency hazard here. Changed to Schedule().
                        // Some options are reverse problem, queue 'water requests'.

                        var bucketWaterLevel = waterLevelComponent[targetBucket.Target];
                        var sourceWaterLevel = waterLevelComponent[targetWaterSource.Target];

                        if (sourceWaterLevel.Level < bucketWaterLevel.Capacity)
                        {
                            state.State = EScooperState.FindWater;
                            break;
                        }

                        var bucketColor = bucketColorComponent[targetBucket.Target];
                        bucketColor.Value = math.float4(config.FullBucketColor.r, config.FullBucketColor.g, config.FullBucketColor.b, 1f);
                        bucketColorComponent[targetBucket.Target] = bucketColor;
                        
                        sourceWaterLevel.Level -= bucketWaterLevel.Capacity;
                        bucketWaterLevel.Level = bucketWaterLevel.Capacity;
                        waterLevelComponent[targetBucket.Target]  = bucketWaterLevel;
                        waterLevelComponent[targetWaterSource.Target] = sourceWaterLevel;
                        state.State = EScooperState.WaitUntilChainStartInRangeAndNotCarrying;
                        break;
                   
                    case EScooperState.WaitUntilChainStartInRangeAndNotCarrying:
                        targetPosition.Target = translationComponent[nextInChain.Next].Position;
                        var nextInChainTargetBucketHasBucket = targetBucketComponent[nextInChain.Next];
                        if (nextInChainTargetBucketHasBucket.Target == Entity.Null)
                        {
                            var chainDistSq = math.distancesq(targetPosition.Target.xz, position.Value.xz);

                            if (chainDistSq < config.MovementTargetReachedThreshold)
                            {
                                state.State = EScooperState.PassBucket;
                            }
                        }
                        break;
                    
                    case EScooperState.PassBucket:
                        if (targetBucket.Target == Entity.Null)
                        {
                            state.State = EScooperState.FindBucket;
                            break;
                        }

                        ecb.AddComponent(entityInQueryIndex, entity, new BucketChangeRequest { Bucket = targetBucket.Target, From = entity, To = nextInChain.Next});
                        break;
                }
            })
                .WithDeallocateOnJobCompletion(waterEntities)
                .WithDeallocateOnJobCompletion(bucketEntities)
                .WithDeallocateOnJobCompletion(bucketAssignments)
                .WithNativeDisableParallelForRestriction(chainComponent)
                .WithNativeDisableParallelForRestriction(bucketColorComponent)
                .WithReadOnly(translationComponent)
                .Schedule(combinedFetchJob);
            
            m_Barrier.AddJobHandleForProducer(Dependency);
        }

        private static Entity FindNearestBucket(ComponentDataFromEntity<LocalToWorld> translationComponent, NativeArray<Entity> potentialEntities, NativeArray<byte> alreadyAssigned, in Translation position, out int foundIndex)
        {
            Entity nearestEntity = default;
            float nearestDistanceSq = float.MaxValue;
            foundIndex = -1;
            
            for (int i = 0; i < potentialEntities.Length; ++i)
            {
                if (alreadyAssigned[i] > 0)
                    continue;
                
                var candidateEntity = potentialEntities[i];
                var waterPosition = translationComponent[candidateEntity];
                var distanceSq = math.distancesq(waterPosition.Position.xz, position.Value.xz);
                if (distanceSq < nearestDistanceSq)
                {
                    nearestDistanceSq = distanceSq;
                    nearestEntity = candidateEntity;
                    foundIndex = i;
                }
            }

            return nearestEntity;
        }
        
        private static Entity FindNearestWaterWithCapacity(ComponentDataFromEntity<LocalToWorld> translationComponent, ComponentDataFromEntity<WaterLevel> waterLevelComponent, NativeArray<Entity> potentialEntities, in Translation position, float capacity)
        {
            Entity nearestEntity = default;
            float nearestDistanceSq = float.MaxValue;
            
            for (int i = 0; i < potentialEntities.Length; ++i)
            {
                var candidateEntity = potentialEntities[i];
                var waterLevel = waterLevelComponent[candidateEntity];
                if (waterLevel.Level < capacity)
                    continue;
                
                var waterPosition = translationComponent[candidateEntity];
                var distanceSq = math.distancesq(waterPosition.Position.xz, position.Value.xz);
                if (distanceSq < nearestDistanceSq)
                {
                    nearestDistanceSq = distanceSq;
                    nearestEntity = candidateEntity;
                }
            }

            return nearestEntity;
        }
    }
}