using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;


[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem : SystemBase
{
    private EntityQuery m_Group;

    [BurstCompile] 
    struct MatrixCompositionJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<RotationY> RotationYTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Size> SizeTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Falling> FallingTypeHandle;
        [ReadOnly] public ComponentTypeHandle<PositionXZ> PositionXYTypeHandle;
        public ComponentTypeHandle<LocalToWorld> LocalToWorldTypeHandle;

        public uint LastSystemVersion;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            var changed =
                chunk.DidOrderChange(LastSystemVersion) ||
                chunk.DidChange(RotationYTypeHandle, LastSystemVersion) ||
                chunk.DidChange(SizeTypeHandle, LastSystemVersion) ||
                chunk.DidChange(FallingTypeHandle, LastSystemVersion) ||
                chunk.DidChange(PositionXYTypeHandle, LastSystemVersion);
            if (!changed)
            {
                return;
            }

            var chunkRotationY = chunk.GetNativeArray(RotationYTypeHandle);
            var hasRotationY = chunk.Has(RotationYTypeHandle);

            var chunkSize = chunk.GetNativeArray(SizeTypeHandle);
            var hasSize = chunk.Has(SizeTypeHandle);

            var chunkFalling = chunk.GetNativeArray(FallingTypeHandle);
            var hasFalling = chunk.Has(FallingTypeHandle);

            var chunkPositionXY = chunk.GetNativeArray(PositionXYTypeHandle);
            if (!chunk.Has(PositionXYTypeHandle))
            {
                throw new Exception("Dieter: this is not right");
            }

            var chunkLocalToWorld = chunk.GetNativeArray(LocalToWorldTypeHandle);

            var count = chunk.Count;

            for (int i = 0; i < count; i++)
            {
                var translationVec = new float3(chunkPositionXY[i].Value.x, 0f, chunkPositionXY[i].Value.y);
                if (hasFalling)
                {
                    translationVec.y = chunkFalling[i].Value;
                }

                var translation = float4x4.Translate(translationVec);

                var rotation = float4x4.identity;
                if (hasRotationY)
                {
                    rotation = float4x4.RotateY(chunkRotationY[i].Value);
                }

                var scale = float4x4.identity;
                if (hasSize)
                {
                    scale = float4x4.Scale(chunkSize[i].Value);
                }

                var m = math.mul(math.mul(translation, rotation), scale);
                chunkLocalToWorld[i] = new LocalToWorld
                {
                    Value = m
                };
            }
        }
    }
    
    protected override void OnCreate()
    {
        m_Group = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[]
            {
                typeof(PositionXZ)
            },
            Options = EntityQueryOptions.FilterWriteGroup
        });
    }

    protected override void OnUpdate()
    {
        var rotationYType = GetComponentTypeHandle<RotationY>(true);
        var sizeType = GetComponentTypeHandle<Size>(true);
        var fallingType = GetComponentTypeHandle<Falling>(true);
        var positionXYType = GetComponentTypeHandle<PositionXZ>(true);
        var localToWorldType = GetComponentTypeHandle<LocalToWorld>(false);
        var job = new MatrixCompositionJob()
        {
            RotationYTypeHandle = rotationYType,
            SizeTypeHandle = sizeType,
            FallingTypeHandle = fallingType,
            PositionXYTypeHandle = positionXYType,
            LocalToWorldTypeHandle = localToWorldType,
            LastSystemVersion = LastSystemVersion
        };
        Dependency = job.Schedule(m_Group, Dependency);
    }
}
