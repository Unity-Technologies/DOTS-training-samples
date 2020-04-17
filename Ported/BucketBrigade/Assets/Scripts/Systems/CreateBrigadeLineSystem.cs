using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CreateBrigadeLineSystem : SystemBase
{
    private EntityQuery mAllRiversQuery;
    private EntityQuery mAllFires;
    EntityQuery mAllBrigade;
    private EndSimulationEntityCommandBufferSystem mEndSimCommandBufferSystem;

    protected override void OnCreate()
    {
        mAllRiversQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<River>(),
            ComponentType.ReadWrite<Translation>(),
            ComponentType.ReadWrite<ValueComponent>());
        mAllFires = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Fire>(),
            ComponentType.ReadOnly<ValueComponent>(),
            ComponentType.ReadOnly<Translation>());

        mAllBrigade = base.GetEntityQuery(ComponentType.Exclude<LineComponent>(), ComponentType.ReadOnly<Brigade>());


        mEndSimCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        mAllRiversQuery.Dispose();
        mAllFires.Dispose();
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
            return;

        if (mAllBrigade.CalculateChunkCount() == 0)
            return;

        var ecb = mEndSimCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var tuningData = GetSingleton<TuningData>();
        var waterPositions = mAllRiversQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var waterPositionHandle);
        var waterEntities = mAllRiversQuery.ToEntityArrayAsync(Allocator.TempJob, out var waterEntityHandle);
        var findWaterJobHandle = JobHandle.CombineDependencies(waterPositionHandle, waterEntityHandle);
        var translations = GetComponentDataFromEntity<Translation>(true);

        var allFireEntities = mAllFires.ToEntityArrayAsync(Allocator.TempJob, out var allFireEntitiesHandle);
        var allFirePositions = mAllFires.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var allFirePositionsHandle);
        var allFireValues = mAllFires.ToComponentDataArrayAsync<ValueComponent>(Allocator.TempJob, out var allFireValuesHandle);
        var combinedDeps = JobHandle.CombineDependencies(allFireEntitiesHandle, allFirePositionsHandle);
        combinedDeps = JobHandle.CombineDependencies(combinedDeps, allFireValuesHandle);
        Dependency = JobHandle.CombineDependencies(combinedDeps, Dependency);
        Dependency = JobHandle.CombineDependencies(Dependency, findWaterJobHandle);

        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities.
            WithAll<Brigade>().
            WithNone<LineComponent>().
            WithDeallocateOnJobCompletion(waterPositions).
            WithDeallocateOnJobCompletion(waterEntities).
            WithDeallocateOnJobCompletion(allFireEntities).
            WithDeallocateOnJobCompletion(allFirePositions).
            WithDeallocateOnJobCompletion(allFireValues).

            WithReadOnly(translations).ForEach((int entityInQueryIndex, Entity e, in DynamicBuffer<ActorElement> actors) =>
        {
            var fillerEntity = actors[1].actor;
            var fillerPosition = translations[fillerEntity];

            float closestWater = float.MaxValue;
            int closestWaterEntity = -1;
            float3 actorPosition = fillerPosition.Value;





            for (int i = 0; i < waterEntities.Length; ++i)
            {
                float distance = math.length(actorPosition - waterPositions[i].Value);
                if (distance < closestWater)
                {
                    closestWaterEntity = i;
                    closestWater = distance;
                }
            }

            //Used to use thrower position, use river for closest
            //var throwerEntity = actors[actors.Length/2 - 1].actor;
            var throwerPosition = translations[waterEntities[closestWaterEntity]];
            float closestFire = float.MaxValue;
            int closestFireEntity = -1;
            actorPosition = throwerPosition.Value;

            for (int i = 0; i < allFireEntities.Length; ++i)
            {
                float distance = math.length(actorPosition - allFirePositions[i].Value);
                if (distance < closestFire && allFireValues[i].Value > tuningData.ValueThreshold)
                {
                    closestFireEntity = i;
                    closestFire = distance;
                }
            }

            if (closestFireEntity == -1)
            {
                return;
            }

            var lineComponent = new LineComponent()
                {
                    start = waterEntities[closestWaterEntity],
                    end = allFireEntities[closestFireEntity]
                };
                ecb.AddComponent(entityInQueryIndex, e, lineComponent);

                float3 startPosition = translations[lineComponent.start].Value;
                float3 endPosition = translations[lineComponent.end].Value;

                float3 dir = math.normalize(endPosition - startPosition);
                float3 perpDir = math.cross(dir, math.up());

                for (int i = 1; i < actors.Length; i++)
                {
                    var actorEntity = actors[i].actor;

                    float t = (i - 1) / (float)(actors.Length - 2);
                    float3 pos = LinePositionFromIndex(t, startPosition, endPosition, perpDir);
                    float3 initPosition = translations[actorEntity].Value;
                    ecb.AddComponent(entityInQueryIndex, actors[i].actor, new Destination() { position = new float3(pos.x, initPosition.y, pos.z) });
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, actorEntity);
                }
            

        }).Schedule();

        //ecb.Playback(EntityManager);
        //ecb.Dispose();

        //waterPositions.Dispose();
        //waterEntities.Dispose();
        //allFireEntities.Dispose();
        //allFirePositions.Dispose();
        //allFireValues.Dispose();

        mEndSimCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    public static float3 LinePositionFromIndex(float t, float3 startPos, float3 endPos, float3 perpendicularOffsetDirection)
    {
        float perpOffset = 5f;

        if (t > .5f)
        {
            t = .5f - (t - .5f);
            perpOffset *= -1;
        }

        t *= 2f;

        perpOffset *= (t * (t + -1f));
        float3 perpVec = perpendicularOffsetDirection * perpOffset;

        return math.lerp(startPos, endPos, t) + perpVec;
    }
}

