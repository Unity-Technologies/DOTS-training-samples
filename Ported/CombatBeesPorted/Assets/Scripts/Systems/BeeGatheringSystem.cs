using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

[WithAll(typeof(BeeStateGathering))]
[BurstCompile]
partial struct BeeGatherJob : IJobEntity
{
    public float GatherRadius;
    [ReadOnly] public StorageInfoFromEntity TargetStorageInfo;
    [ReadOnly] public ComponentDataFromEntity<Translation> TargetTranslationComponentData;
    [ReadOnly] public ComponentDataFromEntity<ResourceStateGrabbable> ResourceStateGrabbableComponentData;
    [ReadOnly] public ComponentDataFromEntity<BlueTeam> BlueTeamComponentData;
    public EntityCommandBuffer ECB;
    public Config config;
    public uint RandomSeed;

    void Execute(Entity entity, in Translation position, ref TargetPosition targetPosition, in EntityOfInterest entityOfInterest)
    {
        Entity targetEntity = entityOfInterest.Value;
        if (TargetStorageInfo.Exists(targetEntity))
        {
            if (TargetTranslationComponentData.HasComponent(targetEntity))
            {
                targetPosition.Value = TargetTranslationComponentData[targetEntity].Value;
                
                if (ResourceStateGrabbableComponentData.IsComponentEnabled(targetEntity))
                {
                    float dist = math.distance(position.Value, targetPosition.Value);

                    if (dist < GatherRadius)
                    {
                        // Set Resource states
                        ECB.SetComponentEnabled<ResourceStateGrabbed>(targetEntity, true);
                        ECB.SetComponentEnabled<ResourceStateGrabbable>(targetEntity, false);

                        // Set Bee states
                        ECB.SetComponentEnabled<BeeStateGathering>(entity, false);
                        ECB.SetComponentEnabled<BeeStateReturning>(entity, true);

                        var rand = Random.CreateFromIndex(RandomSeed + (uint)entity.Index);
                        float randY = rand.NextFloat(position.Value.y + 1.5f, config.PlayVolume.y);
                        float x = config.PlayVolume.x * 2;
                        x *= BlueTeamComponentData.HasComponent(entity) ? -1f : 1f;
                        targetPosition.Value = math.float3(x, randY, position.Value.z);
                    }
                }
            }

        }
        else
        {
            // do busted state stuff
            ECB.SetComponentEnabled<BeeStateGathering>(entity, false);
            ECB.SetComponentEnabled<BeeStateIdle>(entity, true);

            targetPosition.Value = position.Value;
        }
    }
}

[BurstCompile]
public partial struct BeeGatheringSystem : ISystem
{
    private StorageInfoFromEntity storageInfo;
    private ComponentDataFromEntity<Translation> translationComponentData;
    private ComponentDataFromEntity<ResourceStateGrabbable> resourceStateGrabbableComponentData;
    private ComponentDataFromEntity<BlueTeam> blueTeamComponentData;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        storageInfo = state.GetStorageInfoFromEntity();
        translationComponentData = state.GetComponentDataFromEntity<Translation>();
        resourceStateGrabbableComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbable>();
        blueTeamComponentData = state.GetComponentDataFromEntity<BlueTeam>();

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
        blueTeamComponentData.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var gatherJob = new BeeGatherJob()
        {
            GatherRadius = gatherRadius,
            TargetStorageInfo = storageInfo,
            TargetTranslationComponentData = translationComponentData,
            ResourceStateGrabbableComponentData = resourceStateGrabbableComponentData,
            BlueTeamComponentData = blueTeamComponentData,
            ECB = ecb,
            config = config,
            RandomSeed = (uint)UnityEngine.Time.frameCount
        };

        gatherJob.Schedule();
    }
}
