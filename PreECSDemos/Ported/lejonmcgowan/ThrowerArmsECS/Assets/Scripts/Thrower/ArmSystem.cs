using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.SceneManagement;

[UpdateBefore(typeof(ArmRenderUpdateSystem))]
public class ArmSystem : SystemBase
{
    private EntityQuery availableRocksQuery;
    private BeginSimulationEntityCommandBufferSystem beginSimEcbSystem;
    protected override void OnCreate()
    {
        availableRocksQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadOnly<RockRadiusComponentData>()},
            None = new[] {ComponentType.ReadWrite<RockReservedTag>()}
        });

        beginSimEcbSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {

        float t = (float)Time.ElapsedTime;
        float dt = (float)Time.DeltaTime;

        //availableRocksQuery.AddDependency(Dependency); // TODO: ask Aria is this would remove the need to combine the job handles below
        var availableRockEntities = availableRocksQuery.ToEntityArrayAsync(Allocator.TempJob, out var getAvailableRocksJob);
        var ecb = beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();
        var rockReserveJobInputDeps = JobHandle.CombineDependencies(Dependency, getAvailableRocksJob);
        var rockReserveJob = Entities
            .WithReadOnly(availableRockEntities)
            .WithNone<ArmReservedRockComponent>()
            .WithName("RockReserverJob")
            .WithDeallocateOnJobCompletion(availableRockEntities)
            .ForEach((Entity entity, int entityInQueryIndex, in AnchorPosComponentData anchorPos) =>
            {
                for (int i = 0; i < availableRockEntities.Length; i++)
                {
                    var rock = availableRockEntities[i];
                    var rockPos = GetComponent<Translation>(rock).Value;
                    float distSq = math.distancesq(rockPos, anchorPos);
                    float reachSq = 2.0f * 2.0f;
                    if (distSq < reachSq)
                    {
                        ecb.AddComponent(entityInQueryIndex, entity, new ArmReservedRockComponent
                        {
                            value = rock
                        });
                        ecb.AddComponent(entityInQueryIndex,rock,new RockReservedTag());
                    }
                }
            }).ScheduleParallel(rockReserveJobInputDeps);
        
        beginSimEcbSystem.AddJobHandleForProducer(rockReserveJob);
        
        //rockReserveJob.Complete(); //TEMP

        var idleJob = Entities.ForEach((ref ArmIdleTargetComponentData armIdleTarget,
            in AnchorPosComponentData anchorPos) =>
        {
            float time = t + anchorPos.value.x + anchorPos.value.y + anchorPos.value.z;
            
            armIdleTarget = anchorPos +
                             new float3(math.sin(time) * .35f, 1f + math.cos(time * 1.618f) * .5f, 1.5f);

        }).ScheduleParallel(Dependency);

        var debugTargetJob = Entities.
            ForEach((ref ArmGrabTargetComponentData grabTarget,
            ref ArmGrabTimerComponentData grabT,
            in  AnchorPosComponentData anchorPos,
            in ArmUpComponentData armUp) =>
        {
            float3 debugRockPos = new float3(1, 0, 1.5f);
            float debugRadius = 0.25f;
            
            float3 delta = debugRockPos - anchorPos;
            
            float3 flatDelta = delta;
            flatDelta.y = 0f;
            flatDelta = math.normalize(flatDelta);
            
            grabTarget = debugRockPos + armUp.value * debugRadius * .5f -
                             flatDelta * debugRadius * .5f;
            
            grabT = math.sin(0.25f * t);
            grabT = math.clamp(grabT,0f, 1f);
        }).ScheduleParallel(idleJob);


        JobHandle lerpInputJobs = JobHandle.CombineDependencies(idleJob, debugTargetJob);
        
        var ikTargetJob = Entities.ForEach((ref ArmIKTargetComponentData armIKTarget,
            in ArmIdleTargetComponentData armIdleTarget,
            in ArmGrabTargetComponentData armGrabTarget ,
            in ArmGrabTimerComponentData grabT) =>
        {
            armIKTarget.value = math.lerp(armIdleTarget,armGrabTarget,grabT);
        }).ScheduleParallel(lerpInputJobs);
        
        var armIkJob = Entities
            .ForEach((
                ref ArmForwardComponentData armForward,
                ref ArmRightComponentData armRight,
                ref ArmUpComponentData armUp,
                ref DynamicBuffer<ArmJointElementData> armJoints,
                in ArmIKTargetComponentData armTarget,
                in AnchorPosComponentData armAnchorPos
               ) =>
            {
                FABRIK.Solve(armJoints.AsNativeArray().Reinterpret<float3>(), 1f, armAnchorPos.value, armTarget.value,
                    0.1f * armUp.value);
                
                float3 transformRight = new float3(1, 0, 0);
                armForward.value =
                    math.normalize(armJoints[armJoints.Length - 1].value - armJoints[armJoints.Length - 2].value);
                armUp.value = math.normalize(math.cross(armForward.value, transformRight));
                armRight.value = math.cross(armUp.value, armForward.value);
            }).ScheduleParallel(ikTargetJob);

        // Last job becomes the new output dependency
        Dependency = JobHandle.CombineDependencies(rockReserveJob,armIkJob);
    }
}