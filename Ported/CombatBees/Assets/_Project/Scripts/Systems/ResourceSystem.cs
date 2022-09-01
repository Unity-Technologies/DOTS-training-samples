using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(ConstrainToLevelBoundsSystem))]
public partial class ResourceSystem : SystemBase
{
    public NativeArray<int> CellResourceStackCount;
    
    private EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (CellResourceStackCount.IsCreated)
        {
            CellResourceStackCount.Dispose();
        }
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<GameGlobalData>() || 
            !HasSingleton<GameRuntimeData>() ||
            !CellResourceStackCount.IsCreated)
            return;

        EntityCommandBuffer ecb;
        EntityCommandBuffer.ParallelWriter ecbParallel;
        float deltaTime = Time.DeltaTime;
        float time = (float)Time.ElapsedTime;
        GameGlobalData globalData = GetSingleton<GameGlobalData>();
        GameRuntimeData runtimeData = GetSingleton<GameRuntimeData>();
        NativeArray<int> cellResourceStackCount = CellResourceStackCount;
        ComponentDataFromEntity<Translation> translationFromEntity = GetComponentDataFromEntity<Translation>(false);

        // Resource spawning
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .ForEach((Entity entity, ref DynamicBuffer<ResourceSpawnEvent> spawnEvents) =>
            {
                for (int i = 0; i < spawnEvents.Length; i++)
                {
                    Entity newResource = ecb.Instantiate(globalData.ResourcePrefab);
                    ResourceSpawnEvent evnt = spawnEvents[i];
                    ecb.SetComponent(newResource, new Translation { Value = evnt.Position });
                }
                spawnEvents.Clear();
            }).Schedule();

        // Init resource drops by making them remember which cell they are over
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithAll<ResourceInitializeDrop>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Resource resource, ref Translation translation) =>
            {
                int2 cellCoords = runtimeData.GridCharacteristics.GetCellCoordinatesOfPosition(translation.Value);
                float3 cellPos = runtimeData.GridCharacteristics.GetPositionOfCell(cellCoords);
                resource.CellIndex = runtimeData.GridCharacteristics.GetIndexOfCellCoordinates(cellCoords);

                if (resource.CellIndex < 0)
                {
                    // Destroy resource in case it's over an invalid cell index
                    ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                }
                else
                {
                    ecbParallel.RemoveComponent<ResourceInitializeDrop>(entityInQueryIndex, entity);
                    ecbParallel.AddComponent(entityInQueryIndex, entity, new ResourceSnapToCell { SnapStartTime = time, StartPos = translation.Value, TargetPos = cellPos });
                }
            }).ScheduleParallel();
        
        // Make resources snap to their cell lane over time
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in Resource resource, in ResourceSnapToCell resourceSnap) =>
            {
                float timeSinceSnapStart = time - resourceSnap.SnapStartTime;
                float timeRatio = timeSinceSnapStart / globalData.ResourceSnapTime;
                float timeRatioClamped = math.clamp(timeRatio, 0f, 1f);

                translation.Value.x = math.lerp(resourceSnap.StartPos.x, resourceSnap.TargetPos.x, timeRatioClamped); 
                translation.Value.z = math.lerp(resourceSnap.StartPos.z, resourceSnap.TargetPos.z, timeRatioClamped);

                if (timeRatio >= 1f)
                {
                    translation.Value.x = resourceSnap.TargetPos.x;
                    translation.Value.z = resourceSnap.TargetPos.z;
                    ecbParallel.RemoveComponent<ResourceSnapToCell>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        // Make unsettled carrier-less resources fall with gravity
        Entities
            .WithNone<ResourceCarrier>()
            .WithNone<ResourceSettled>()
            .ForEach((ref Translation translation, ref Resource resource) =>
            {
                resource.Velocity += math.up() * globalData.Gravity * deltaTime;
                translation.Value += resource.Velocity * deltaTime;
            }).ScheduleParallel();
        
        // Detect reaching ground or stacking
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithNone<ResourceCarrier>()
            .WithNone<ResourceSettled>()
            .ForEach((Entity entity, ref Translation translation, ref Resource resource) =>
            {
                int currentStackCount = cellResourceStackCount[resource.CellIndex];
                float expectedFloorHeight = currentStackCount * globalData.ResourceHeight;
                
                if (translation.Value.y <= expectedFloorHeight)
                {
                    ecb.AddComponent(entity, new ResourceSettled());
                    
                    translation.Value.y = expectedFloorHeight;
                    resource.Velocity = default;
                    resource.StackIndex = currentStackCount;

                    cellResourceStackCount[resource.CellIndex] = currentStackCount + 1;
                    
                    // TODO: if landed in a team zone, destroy resources, spawn particles, and spawn bees
                }
            }).Schedule(); // This one is not parallel because 2 resources may want to write to same index of NativeArray
        
        // Make carried resources follow their carrier
        Entities
            .WithNativeDisableParallelForRestriction(translationFromEntity) // safe because we only ever write to self entity
            .ForEach((Entity entity, ref Resource resource, in ResourceCarrier carrier) => 
            {
                if (translationFromEntity.HasComponent(carrier.Carrier))
                {
                    Translation selfTranslation = translationFromEntity[entity];
                    
                    float3 initialPos = selfTranslation.Value;
                    float3 carrierPos = translationFromEntity[carrier.Carrier].Value;
                    float3 targetPos = carrierPos - (math.up() * globalData.ResourceCarryOffset);
                    selfTranslation.Value = math.lerp(selfTranslation.Value, targetPos, globalData.ResourceCarryStiffness * deltaTime);
                    resource.Velocity = (selfTranslation.Value - initialPos) / deltaTime;

                    translationFromEntity[entity] = selfTranslation;
                }
            }).ScheduleParallel();
        
        // Make settled resources remove themselves from stacks when carried
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithAll<ResourceCarrier>()
            .WithAll<ResourceSettled>()
            .ForEach((Entity entity, ref Resource resource) => 
            {
                int currentStackCount = cellResourceStackCount[resource.CellIndex];
                cellResourceStackCount[resource.CellIndex] = math.max(0, currentStackCount - 1);
                ecb.RemoveComponent<ResourceSettled>(entity);
            }).Schedule();
        
        // Drop resource if carrier died
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Resource resource, in ResourceCarrier carrier) => 
            {
                if (!HasComponent<Bee>(carrier.Carrier))
                {
                    ecbParallel.RemoveComponent<ResourceCarrier>(entityInQueryIndex, entity);
                    ecbParallel.AddComponent(entityInQueryIndex, entity, new ResourceInitializeDrop());
                }
            }).ScheduleParallel();
        
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
