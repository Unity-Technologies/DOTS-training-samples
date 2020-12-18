using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;

[BurstCompile]
public class BucketSystem : SystemBase
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
	NativeArray<int2> m_NeighborOffsets;

    protected override void OnCreate()
    {
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Fetcher>()));
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Thrower>()));
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Bucket>()));

        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

		m_NeighborOffsets = new NativeArray<int2>(8, Allocator.Persistent);
		NativeArray<int2>.Copy(new [] {new int2(+0, -1),
			new int2(+1, -1),
			new int2(+1, +0),
			new int2(+1, +1),
			new int2(+0, +1),
			new int2(-1, +1),
			new int2(-1, +0),
			new int2(-1, -1)}, m_NeighborOffsets);
    }

    protected override void OnDestroy()
    {
		m_NeighborOffsets.Dispose();
    }

    protected override void OnUpdate()
    {
        NativeArray<float2> throwerCoords = BucketTeamCollectInfoSystem.s_ThrowerCoords;
        NativeArray<float2> fetcherCoords = BucketTeamCollectInfoSystem.s_FetcherCoords;
        NativeArray<float2> teamDirection = BucketTeamCollectInfoSystem.s_TeamDirection;

        SplayParams splayParams = new SplayParams
        {
            SplayMin   = VisualConfig.kSplayMin,
            SplayMax   = VisualConfig.kSplayMax,
            SplayStart = VisualConfig.kSplayStart,
            SplayEnd   = VisualConfig.kSplayEnd
        };

		float currentDeltaTime = Time.DeltaTime;
        float bucketSpeed = FireSimConfig.kBucketSpeed;

        Entities
            .WithReadOnly(fetcherCoords)
            .WithReadOnly(throwerCoords)
            .WithReadOnly(teamDirection)
            .WithNativeDisableContainerSafetyRestriction(fetcherCoords)
            .WithNativeDisableContainerSafetyRestriction(throwerCoords)
            .WithNativeDisableContainerSafetyRestriction(teamDirection)
            .ForEach((ref Position position, ref Bucket bucket, in BucketOwner bucketOwner, in WaterLevel waterLevel) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    int teamIndex = bucketOwner.AsCohortIndex();
                    float bias = waterLevel.Value < 0.5f ? 1.0f : -1.0f; // empty buckets carried by empty bot who is biased to +1
                    float2 dir = teamDirection[teamIndex];

                    float3 pos;
                    BucketTeamSystem.UpdateTranslation(out pos, fetcherCoords[teamIndex], throwerCoords[teamIndex], dir, bucket.LinearT, bias, splayParams);
                    position.coord = new float2(pos.x, pos.z);
                }
            }).ScheduleParallel();

        Entities
            .ForEach((ref Bucket bucket, in BucketOwner bucketOwner, in WaterLevel waterLevel) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    float direction = waterLevel.Value < 0.5f ? -1.0f : 1.0f; // full -> forward, empty -> back
                    bucket.LinearT += direction * currentDeltaTime * bucketSpeed;
                }
            }).ScheduleParallel();

        var boardEntity = GetSingletonEntity<BoardElement>();
        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
		var neighborOffsets = m_NeighborOffsets;
        var xDim = FireSimConfig.xDim;
		var yDim = FireSimConfig.yDim;

        Entities
            .WithReadOnly(neighborOffsets)
            .ForEach((int entityInQueryIndex, ref Bucket bucket, ref BucketOwner bucketOwner, in Position position) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    if (bucket.LinearT < 0.0f || bucket.LinearT > 1.0f)
                    {
                        // unassign
                        bucketOwner.Value = 0;

                        // append this cell
                        int2 coord = new int2(position.coord);
                        int xc = math.max(math.min(coord.x, xDim-1), 0);
                        int yc = math.max(math.min(coord.y, yDim-1), 0);
                        int index = yc * xDim + xc;
                        ecb.AppendToBuffer<DouseElement>(entityInQueryIndex, boardEntity, new DouseElement { Value = index });

                        for (int i=0; i<8; ++i)
                        {
                            int2 neighborCoord = coord + neighborOffsets[i];
                            int nxc = math.max(math.min(neighborCoord.x, xDim-1), 0);
                            int nyc = math.max(math.min(neighborCoord.y, yDim-1), 0);
                            int neighborIndex = nyc * xDim + nxc;
                            ecb.AppendToBuffer<DouseElement>(entityInQueryIndex, boardEntity, new DouseElement { Value = neighborIndex });
                        }
                    }
                }
            }).ScheduleParallel();

        Entities
            .WithAll<Bucket>()
            .ForEach((ref LocalToWorld localToWorld, in Position position) =>
            {
                localToWorld.Value.c3 = new float4(position.coord.x, 1.0f, position.coord.y, localToWorld.Value.c3.w);
            }).Schedule();
    }
}
