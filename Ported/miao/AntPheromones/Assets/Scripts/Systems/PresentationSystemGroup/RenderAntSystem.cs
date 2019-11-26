using System.Collections.Generic;
using AntPheromones_ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Profiling;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class RenderAntSystem : JobComponentSystem
{
    private EntityQuery _antQuery;
    private EntityQuery _antSharedRenderingQuery;

    private readonly List<Matrix4x4[]> _matrices = new List<Matrix4x4[]>();
    private readonly List<Vector4[]> _colours = new List<Vector4[]>();
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

        NativeArray<Matrix4x4> localToWorlds;
        NativeArray<Vector4> colours;
        
        using (new ProfilerMarker("ToComponentArray").Auto())
        {
            localToWorlds = this._antQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob).Reinterpret<Matrix4x4>();
            colours = this._antQuery.ToComponentDataArray<Colour>(Allocator.TempJob).Reinterpret<Vector4>();
        }

        for (int batch = 0; batch < numBatches; batch++)
        {
            int batchSize = batch == numBatches - 1 ? modulo : MaxBatchSize;
            
            NativeArray<Matrix4x4>.Copy(
                src: localToWorlds.GetSubArray(start: batch * MaxBatchSize, length: batchSize),
                dst: this._matrices[batch],
                length: batchSize);
            NativeArray<Vector4>.Copy(
                src: colours.GetSubArray(start: batch * MaxBatchSize, length: batchSize),
                dst: this._colours[batch],
                length: batchSize);
            
            this._materialPropertyBlocks[batch].SetVectorArray(ColourId, this._colours[batch]);
            
            var sharedRenderingData =
                EntityManager.GetSharedComponentData<AntSharedRendering>(
                    this._antSharedRenderingQuery.GetSingletonEntity());

            Graphics.DrawMeshInstanced(
                sharedRenderingData.Mesh, 
                submeshIndex: 0, 
                material: sharedRenderingData.Material, 
                matrices: this._matrices[batch],
                count:batchSize,
                this._materialPropertyBlocks[batch]);
        }

        localToWorlds.Dispose();
        colours.Dispose();
        
        return default;
    }
}