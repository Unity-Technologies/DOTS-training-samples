using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AntPheromones_ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class RenderAntSystem : JobComponentSystem
{
    private EntityQuery _antQuery;
    private EntityQuery _antSharedRenderingQuery;

    private readonly List<Matrix4x4[]> _matrices = new List<Matrix4x4[]>();
    private readonly List<Vector4[]> _colours = new List<Vector4[]>();
    private readonly List<GCHandle> _gcHandlesToFree = new List<GCHandle>();
    private readonly List<MaterialPropertyBlock> _materialPropertyBlocks = new List<MaterialPropertyBlock>();

    private static readonly int ColourId = Shader.PropertyToID("_Color");
    private const int MaxBatchSize = 1023;

    protected override void OnCreate()
    {
        base.OnCreate();

        this._antQuery = GetEntityQuery(
            ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<Colour>()
        );
        this._antSharedRenderingQuery = GetEntityQuery(
            ComponentType.ReadOnly<AntSharedRendering>()
        );
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int numAnts = this._antQuery.CalculateEntityCount();

        int modulo = numAnts % MaxBatchSize;
        int numBatches = numAnts / MaxBatchSize + (modulo > 0 ? 1 : 0);

        while (this._matrices.Count < numBatches)
        {
            this._matrices.Add(new Matrix4x4[MaxBatchSize]);

            var batchColors = new Vector4[MaxBatchSize];
            this._colours.Add(batchColors);

            var propertyBlock = new MaterialPropertyBlock();
            this._materialPropertyBlocks.Add(propertyBlock);

            propertyBlock.SetVectorArray(nameID: ColourId, batchColors);
        }

        var antRenderer = 
            EntityManager.GetSharedComponentData<AntSharedRendering>(this._antSharedRenderingQuery.GetSingletonEntity());
        
        var matrices = new NativeArray<IntPtr>(numBatches, Allocator.Temp);
        var colors = new NativeArray<IntPtr>(numBatches, Allocator.Temp);
        
        unsafe
        {
            for (int batch = 0; batch < numBatches; batch++)
            {
                GCHandle matricesHandle = GCHandle.Alloc(this._matrices[batch], GCHandleType.Pinned);
                GCHandle coloursHandle = GCHandle.Alloc(this._colours[batch], GCHandleType.Pinned);
                
                matrices[batch] = matricesHandle.AddrOfPinnedObject();
                colors[batch] = coloursHandle.AddrOfPinnedObject();
                
                this._gcHandlesToFree.Add(matricesHandle);
                this._gcHandlesToFree.Add(coloursHandle);
            }

            new Job
            {
                Colours = (Vector4**)colors.GetUnsafePtr(),
                Matrices = (Matrix4x4**)matrices.GetUnsafePtr(),
            }.Schedule(this, inputDeps).Complete();

            foreach (GCHandle gcHandle in this._gcHandlesToFree)
            {
                gcHandle.Free();
            }
            
            this._gcHandlesToFree.Clear();
        }

        for (int batch = 0; batch < numBatches; batch++)
        {
            int batchSize = batch == numBatches - 1 ? modulo : MaxBatchSize;
            this._materialPropertyBlocks[batch].SetVectorArray(ColourId, this._colours[batch]);
            
            Graphics.DrawMeshInstanced(
                antRenderer.Mesh,
                submeshIndex:0,
                antRenderer.Material,
                this._matrices[batch],
                count: batchSize,
                this._materialPropertyBlocks[batch]
            );
        }
        return default;
    }
    
    [BurstCompile]
    unsafe struct Job : IJobForEachWithEntity<LocalToWorld, Colour>
    {
        [NativeDisableUnsafePtrRestriction] public Matrix4x4** Matrices;
        [NativeDisableUnsafePtrRestriction] public Vector4** Colours;
        
        public void Execute(
            Entity entity, 
            int index,
            [ReadOnly] ref LocalToWorld localToWorld,
            [ReadOnly] ref Colour colour)
        {
            int batch = index / MaxBatchSize;
            int modulo = index % MaxBatchSize;
            
            this.Matrices[batch][modulo] = localToWorld.Value;
            this.Colours[batch][modulo] = colour.Value;
        }
    }
}