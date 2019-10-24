using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public struct ArmBookingItem
{
    public Entity arm;
    public Entity rock;
    public Entity can;
}

public class ArmBookingSystem : JobComponentSystem
{
    [BurstCompile]
    struct ArmBookingSystemJob : IJobForEachWithEntity_EBC<BoneJoint, ArmTarget>
    {
        [ReadOnly]public NativeArray<Entity> rockEntities;
        [ReadOnly] public NativeArray<Entity> canEntities;
        [ReadOnly] public NativeArray<Translation> rockPos;
        [ReadOnly] public NativeArray<Translation> canPos;
        public NativeHashMap<Entity, ArmBookingItem>.ParallelWriter chosenOnes;
        [ReadOnly]public float reachRockMaxDistance;
        [ReadOnly]public float throwRockMaxDistance;
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints, ref ArmTarget armTarget)
        {
            if (armTarget.TargetRock != Entity.Null) 
                return;

            int nearestRockIdx = FindNearest(boneJoints[0].JointPos,ref rockPos, reachRockMaxDistance);
            if (nearestRockIdx == -1) 
                return;

            int nearestCanIdx = FindNearest(boneJoints[0].JointPos, ref canPos, throwRockMaxDistance);
            if (nearestCanIdx == -1) 
                return;

            armTarget.TargetRock = rockEntities[nearestRockIdx];
            armTarget.TargetCan = canEntities[nearestCanIdx];
            chosenOnes.TryAdd(armTarget.TargetRock, new ArmBookingItem
            {
                rock = rockEntities[nearestRockIdx],
                can = canEntities[nearestCanIdx],
                arm = entity
            });
        }

        int FindNearest(float3 translationValue, ref NativeArray<Translation> targets, float maxDistance = float.MaxValue)
        {
            float nearestDistance = float.MaxValue;
            int resultIdx = -1;
            for (int i = 1; i < targets.Length; i++)
            {
                float dist = math.lengthsq(translationValue - targets[i].Value);
                if (dist < maxDistance && dist < nearestDistance)
                {
                    nearestDistance = dist;
                    resultIdx = i;
                }
            }
            return resultIdx;
        }
    }

    EntityQuery m_ArmGroup;
    EntityQuery m_RockGroup;
    EntityQuery m_CanGroup;

    protected override void OnCreate()
    {
        m_ArmGroup = GetEntityQuery( ComponentType.ReadWrite<BoneJoint>(), ComponentType.ReadWrite<ArmTarget>());
        m_RockGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<RockTag>(), ComponentType.ReadOnly<Reserved>());
        Reserved r = new Reserved() { reserved = false };
        m_RockGroup.SetFilter(r);
        m_CanGroup = GetEntityQuery( ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<TinCanTag>(), ComponentType.ReadOnly<Reserved>());
        m_CanGroup.SetFilter(r);
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var rockCount = m_RockGroup.CalculateEntityCount();
        var canCount = m_CanGroup.CalculateEntityCount();
        if (rockCount == 0 || canCount == 0)
            return inputDependencies;

        var canPos = m_CanGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
        var rockPos = m_RockGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
        var canEntities = m_CanGroup.ToEntityArray(Allocator.TempJob);
        var rockEntities = m_RockGroup.ToEntityArray(Allocator.TempJob);
        var chosenOnes = new NativeHashMap<Entity, ArmBookingItem>(rockCount, Allocator.TempJob);

        //Booking
        var job = new ArmBookingSystemJob();
        job.canEntities = canEntities;
        job.rockEntities = rockEntities;
        job.canPos = canPos;
        job.rockPos = rockPos;
        //job.reachDistance = 1.8f;
        job.reachRockMaxDistance = 5f;
        job.throwRockMaxDistance = 1000f;
        job.chosenOnes = chosenOnes.AsParallelWriter();
        JobHandle selectionJh = job.Schedule(m_ArmGroup, inputDependencies);
        selectionJh.Complete();

        //Checking for duplicate
        var bookingInfos = chosenOnes.GetValueArray(Allocator.Temp);
        List<Entity> alreadyUsedCan = new List<Entity>(bookingInfos.Length);
        List<Entity> alreadyUsedRock = new List<Entity>(bookingInfos.Length);
        for (int i = 0; i < bookingInfos.Length; i++)
        {
            if (alreadyUsedCan.Contains(bookingInfos[i].can) || alreadyUsedRock.Contains(bookingInfos[i].rock))
            {
                ArmTarget target = EntityManager.GetComponentData<ArmTarget>(bookingInfos[i].arm);
                target.TargetCan = Entity.Null;
                target.TargetRock = Entity.Null;
                EntityManager.SetComponentData(bookingInfos[i].arm, target);
            }
            else
            {
                alreadyUsedCan.Add(bookingInfos[i].can);
                alreadyUsedRock.Add(bookingInfos[i].rock);
                EntityManager.SetSharedComponentData(bookingInfos[i].rock, new Reserved { reserved = true });
                EntityManager.SetSharedComponentData(bookingInfos[i].can, new Reserved { reserved = true });

                //For debug - to be removed
                Translation pos = EntityManager.GetComponentData<Translation>(bookingInfos[i].can);
                EntityManager.SetComponentData(bookingInfos[i].rock, new ForceThrow() { target = pos.Value });
                ArmTarget target = EntityManager.GetComponentData<ArmTarget>(bookingInfos[i].arm);
                target.TargetCan = Entity.Null;
                target.TargetRock = Entity.Null;
                EntityManager.SetComponentData(bookingInfos[i].arm, target);
            }
        }

        bookingInfos.Dispose();
        canPos.Dispose();
        rockPos.Dispose();
        canEntities.Dispose();
        rockEntities.Dispose();
        chosenOnes.Dispose();

        return inputDependencies;
    }
}