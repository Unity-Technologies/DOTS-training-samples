using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;


[BurstCompile]
struct MatrixCompositionJob : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<Direction> RotationYTypeHandle;
    [ReadOnly] public ComponentTypeHandle<Size> SizeTypeHandle;
    [ReadOnly] public ComponentTypeHandle<SizeGrown> SizeGrownTypeHandle;
    [ReadOnly] public ComponentTypeHandle<Falling> FallingTypeHandle;
    [ReadOnly] public ComponentTypeHandle<PositionXZ> PositionXZTypeHandle;
    public ComponentTypeHandle<LocalToWorld> LocalToWorldTypeHandle;

    public uint LastSystemVersion;

    public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
    {
        var changed =
            chunk.DidOrderChange(LastSystemVersion) ||
            chunk.DidChange(RotationYTypeHandle, LastSystemVersion) ||
            chunk.DidChange(SizeTypeHandle, LastSystemVersion) ||
            chunk.DidChange(SizeGrownTypeHandle, LastSystemVersion) ||
            chunk.DidChange(FallingTypeHandle, LastSystemVersion) ||
            chunk.DidChange(PositionXZTypeHandle, LastSystemVersion);
        if (!changed)
        {
            return;
        }

        var chunkRotationY = chunk.GetNativeArray(RotationYTypeHandle);
        var hasRotationY = chunk.Has(RotationYTypeHandle);

        var chunkSize = chunk.GetNativeArray(SizeTypeHandle);
        var hasSize = chunk.Has(SizeTypeHandle);

        var chunkSizeGrown = chunk.GetNativeArray(SizeGrownTypeHandle);
        var hasSizeGrown = chunk.Has(SizeGrownTypeHandle);

        var chunkFalling = chunk.GetNativeArray(FallingTypeHandle);
        var hasFalling = chunk.Has(FallingTypeHandle);

        var chunkPositionXZ = chunk.GetNativeArray(PositionXZTypeHandle);
        if (!chunk.Has(PositionXZTypeHandle))
        {
            throw new Exception("Dieter: this is not right");
        }

        var chunkLocalToWorld = chunk.GetNativeArray(LocalToWorldTypeHandle);
        if (!chunk.Has(LocalToWorldTypeHandle))
        {
            throw new Exception("Dieter: this is not right");
        }

        var count = chunk.Count;

        for (int i = 0; i < count; i++)
        {
            var translationVec = new float3(chunkPositionXZ[i].Value.x, 0f, chunkPositionXZ[i].Value.y);
            if (hasFalling)
            {
                translationVec.y = chunkFalling[i].Value;
            }

            var translation = float4x4.Translate(translationVec);

            var rotation = float4x4.identity;
            if (hasRotationY)
            {
                var rv = 0.0f;
                switch (chunkRotationY[i].Value)
                {
                    case Direction.Attributes.Up:
                        rv = 0;
                        break;
                    case Direction.Attributes.Down:
                        rv = 3.14f;
                        break;
                    case Direction.Attributes.Right:
                        rv = 1.57f;
                        break;
                    case Direction.Attributes.Left:
                        rv = 4.71f;
                        break;
                }

                rotation = float4x4.RotateY(rv);
            }

            var scale = float4x4.identity;
            if (hasSize)
            {
                var s = chunkSize[i].Value;
                if (hasSizeGrown)
                {
                    s += chunkSizeGrown[i].Grow;
                }

                scale = float4x4.Scale(s);
            }

            var m = math.mul(math.mul(translation, rotation), scale);
            chunkLocalToWorld[i] = new LocalToWorld
            {
                Value = m
            };
        }
    }
}
    

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        m_Group = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[]
            {
                ComponentType.ReadOnly<PositionXZ>(),
                ComponentType.ReadWrite<LocalToWorld>(), 
            },
            None = new[]
            {
                ComponentType.ReadOnly<Static>(), 
            }
        });
    }

    protected EntityQuery m_Group;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var rotationYType = GetComponentTypeHandle<Direction>(true);
        var sizeType = GetComponentTypeHandle<Size>(true);
        var sizeGrownType = GetComponentTypeHandle<SizeGrown>(true);
        var fallingType = GetComponentTypeHandle<Falling>(true);
        var positionXZType = GetComponentTypeHandle<PositionXZ>(true);
        var localToWorldType = GetComponentTypeHandle<LocalToWorld>(false);
        var job = new MatrixCompositionJob()
        {
            RotationYTypeHandle = rotationYType,
            SizeTypeHandle = sizeType,
            SizeGrownTypeHandle = sizeGrownType,
            FallingTypeHandle = fallingType,
            PositionXZTypeHandle = positionXZType,
            LocalToWorldTypeHandle = localToWorldType,
            LastSystemVersion = LastSystemVersion
        };
        return job.Schedule(m_Group, inputDeps);
    }
}

public struct StaticDone : IComponentData {}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TransformSystemStatic : JobComponentSystem
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_Group = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[]
            {
                ComponentType.ReadOnly<PositionXZ>(),
                ComponentType.ReadWrite<LocalToWorld>(), 
                ComponentType.ReadOnly<Static>(), 
            },
            None = new[]
            {
                ComponentType.ReadOnly<StaticDone>(), 
            }
        });
        
        m_CommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    
    protected EntityQuery m_Group;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var rotationYType = GetComponentTypeHandle<Direction>(true);
        var sizeType = GetComponentTypeHandle<Size>(true);
        var sizeGrownType = GetComponentTypeHandle<SizeGrown>(true);
        var fallingType = GetComponentTypeHandle<Falling>(true);
        var positionXZType = GetComponentTypeHandle<PositionXZ>(true);
        var localToWorldType = GetComponentTypeHandle<LocalToWorld>(false);
        var job = new MatrixCompositionJob()
        {
            RotationYTypeHandle = rotationYType,
            SizeTypeHandle = sizeType,
            SizeGrownTypeHandle = sizeGrownType,
            FallingTypeHandle = fallingType,
            PositionXZTypeHandle = positionXZType,
            LocalToWorldTypeHandle = localToWorldType,
            LastSystemVersion = LastSystemVersion
        };
        var dep = job.Schedule(m_Group, inputDeps);
        
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var job2 = Entities
            .WithAll<Static>()
            .WithNone<StaticDone>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
               ecb.AddComponent<StaticDone>(entityInQueryIndex, entity);
            });
            
        var dep2 = job2.Schedule(dep);
        m_CommandBufferSystem.AddJobHandleForProducer(dep2);
        return dep2;

    }
}