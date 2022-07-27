using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[WithAll(typeof(BeeStateGathering))]
[CreateAfter(typeof(WorldSetupSystem))]
[BurstCompile]
partial struct BeeGatherJob : IJobEntity
{
    public float GatherRadius;
    [ReadOnly] public StorageInfoFromEntity TargetStorageInfo;
    [ReadOnly] public ComponentDataFromEntity<Translation> TargetTranslationComponentData;
    public ComponentDataFromEntity<ResourceStateGrabbable> ResourceStateGrabbableComponentData;
    public ComponentDataFromEntity<ResourceStateGrabbed> ResourceStateGrabbedComponentData;

    void Execute(Entity e, in Translation position, ref TargetPosition targetPosition, in EntityOfInterest entityOfInterest)
    {
        Entity targetEntity = entityOfInterest.Value;

        if (TargetStorageInfo.Exists(targetEntity))
        {
            if (ResourceStateGrabbableComponentData.IsComponentEnabled(targetEntity))
            {
                if (TargetTranslationComponentData.HasComponent(targetEntity))
                {
                    targetPosition.Value = TargetTranslationComponentData[targetEntity].Value;

                    float dist = math.distance(position.Value, targetPosition.Value);

                    if (dist < GatherRadius)
                    {
                        // 

                        // set states
                    }
                }
            }
        }
        else
        {
            // do busted state stuff
        }
    }
}

[BurstCompile]
public partial struct BeeGatheringSystem : ISystem
{
    private StorageInfoFromEntity storageInfo;
    private ComponentDataFromEntity<Translation> translationComponentData;
    private ComponentDataFromEntity<ResourceStateGrabbable> resourceStateGrabbableComponentData;
    private ComponentDataFromEntity<ResourceStateGrabbed> resourceStateGrabbedComponentData;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        storageInfo = state.GetStorageInfoFromEntity();
        translationComponentData = state.GetComponentDataFromEntity<Translation>();
        resourceStateGrabbableComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbable>();
        resourceStateGrabbedComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbed>();

        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        float gatherRadius = config.InteractionDistance;

        storageInfo.Update(ref state);
        translationComponentData.Update(ref state);
        resourceStateGrabbableComponentData.Update(ref state);
        resourceStateGrabbedComponentData.Update(ref state);

        var gatherJob = new BeeGatherJob()
        {
            GatherRadius = gatherRadius,
            TargetStorageInfo = storageInfo,
            TargetTranslationComponentData = translationComponentData,
            ResourceStateGrabbableComponentData = resourceStateGrabbableComponentData,
            ResourceStateGrabbedComponentData = resourceStateGrabbedComponentData
        };

        gatherJob.Schedule();
    }
}
