using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ParticleRenderSystem : JobComponentSystem
    {
        EntityQuery m_Renderables;
        EntityQuery m_ParticleSetup;
        readonly List<Matrix4x4[]> m_Matrices = new List<Matrix4x4[]>();
        readonly List<Vector4[]> m_Colors = new List<Vector4[]>();
        readonly List<MaterialPropertyBlock> m_PropertyBlocks = new List<MaterialPropertyBlock>();
        static readonly int k_Color = Shader.PropertyToID("_Color");

        const int k_MaxBatchSize = 1023;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Renderables = GetEntityQuery(
                ComponentType.ReadOnly<LocalToWorldComponent>(),
                ComponentType.ReadOnly<RenderColorComponent>()
            );
            m_ParticleSetup = GetEntityQuery(
                ComponentType.ReadOnly<ParticleSetupComponent>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var particleSetup = EntityManager.GetSharedComponentData<ParticleSetupComponent>(m_ParticleSetup.GetSingletonEntity());

            var numRenderables = m_Renderables.CalculateEntityCount();
            using (var matrices = new NativeArray<Matrix4x4>(numRenderables, Allocator.TempJob))
            using (var colors = new NativeArray<Vector4>(numRenderables, Allocator.TempJob))
            {
                var renderDataJob = new CollectRenderDataJob
                {
                    Matrices = matrices,
                    Colors = colors,
                }.Schedule(this, inputDeps);

                // allocate batch data
                int numFullBatches = numRenderables / k_MaxBatchSize;
                int remaining = numRenderables % k_MaxBatchSize;
                int totalBatches = numFullBatches + (remaining > 0 ? 1 : 0);
                while (m_Matrices.Count < totalBatches)
                {
                    m_Matrices.Add(new Matrix4x4[k_MaxBatchSize]);
                    var batchColors = new Vector4[k_MaxBatchSize];
                    m_Colors.Add(batchColors);
                    var propertyBlock = new MaterialPropertyBlock();
                    m_PropertyBlocks.Add(propertyBlock);
                    propertyBlock.SetVectorArray(k_Color, batchColors);
                }

                renderDataJob.Complete();
                
                // Do the actual rendering:
                //  * copy over matrices
                //  * copy over colors
                //  * use Graphics.DrawMeshInstanced
                unsafe
                {
                    var pSrcMatrices = (Matrix4x4*)matrices.GetUnsafeReadOnlyPtr();
                    var pSrcColors = (Vector4*)colors.GetUnsafeReadOnlyPtr();
                    for (int batch = 0; batch < numFullBatches; batch++)
                    {
                        int batchSize = (batch == numFullBatches - 1) ? remaining : k_MaxBatchSize;
                        fixed (Matrix4x4* pDstMatrices = m_Matrices[batch])
                        fixed (Vector4* pDstColors = m_Colors[batch])
                        {
                            UnsafeUtility.MemCpy(pDstMatrices, pSrcMatrices, batchSize * sizeof(Matrix4x4));
                            UnsafeUtility.MemCpy(pDstColors, pSrcColors, batchSize * sizeof(Vector4));
                            pSrcMatrices += batchSize;
                            pSrcColors += batchSize;
                        }
                        m_PropertyBlocks[batch].SetVectorArray(k_Color, m_Colors[batch]);
                        Graphics.DrawMeshInstanced(
                            particleSetup.Mesh,
                            0,
                            particleSetup.Material,
                            m_Matrices[batch],
                            batchSize,
                            m_PropertyBlocks[batch]
                        );
                    }
                }
            }

            return default(JobHandle);
        }

        [BurstCompile]
        struct CollectRenderDataJob : IJobForEachWithEntity<LocalToWorldComponent, RenderColorComponent>
        {
            [NativeDisableParallelForRestriction]
            public NativeArray<Matrix4x4> Matrices;
            [NativeDisableParallelForRestriction]
            public NativeArray<Vector4> Colors;

            public void Execute(Entity entity, int index,
                [ReadOnly] ref LocalToWorldComponent localToWorld,
                [ReadOnly] ref RenderColorComponent color)
            {
                Matrices[index] = localToWorld.Value;
                Colors[index] = color.Value;
            }
        }
    }
}
