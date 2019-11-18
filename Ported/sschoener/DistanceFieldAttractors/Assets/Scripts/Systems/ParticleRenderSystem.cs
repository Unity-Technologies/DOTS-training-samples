using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ParticleRenderSystem : JobComponentSystem
    {
        EntityQuery m_Renderables;
        EntityQuery m_ParticleSetup;
        readonly List<Matrix4x4[]> m_Matrices = new List<Matrix4x4[]>();
        readonly List<Vector4[]> m_Colors = new List<Vector4[]>();
        readonly List<GCHandle> m_Handles = new List<GCHandle>();
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
            int remaining = numRenderables % k_MaxBatchSize;
            int numBatches = numRenderables / k_MaxBatchSize + (remaining > 0 ? 1 : 0);

            // allocate batch data
            while (m_Matrices.Count < numBatches)
            {
                m_Matrices.Add(new Matrix4x4[k_MaxBatchSize]);
                var batchColors = new Vector4[k_MaxBatchSize];
                m_Colors.Add(batchColors);
                var propertyBlock = new MaterialPropertyBlock();
                m_PropertyBlocks.Add(propertyBlock);
                propertyBlock.SetVectorArray(k_Color, batchColors);
            }
            
            NativeArray<IntPtr> matrices  = new NativeArray<IntPtr>(numBatches, Allocator.Temp);
            NativeArray<IntPtr> colors = new NativeArray<IntPtr>(numBatches, Allocator.Temp);
            unsafe
            {
                for (int batch = 0; batch < numBatches; batch++)
                {
                    var matricesHandle = GCHandle.Alloc(m_Matrices[batch], GCHandleType.Pinned);
                    var colorsHandle = GCHandle.Alloc(m_Colors[batch], GCHandleType.Pinned);
                    m_Handles.Add(matricesHandle);
                    m_Handles.Add(colorsHandle);
                    matrices[batch] = matricesHandle.AddrOfPinnedObject();
                    colors[batch] = colorsHandle.AddrOfPinnedObject();
                }

                new CollectRenderDataJob()
                {
                    Colors = (Vector4**)colors.GetUnsafePtr(),
                    Matrices = (Matrix4x4**)matrices.GetUnsafePtr(),
                }.Schedule(this, inputDeps).Complete();

                for (int batch = 0; batch < numBatches; batch++)
                {
                    m_Handles[2 * batch + 0].Free();
                    m_Handles[2 * batch + 1].Free();
                }

                m_Handles.Clear();
                
            }

            using (new ProfilerMarker("DrawMeshInstanced").Auto())
            {
                for (int batch = 0; batch < numBatches; batch++)
                {
                    int batchSize = (batch == numBatches - 1) ? remaining : k_MaxBatchSize;
                    m_PropertyBlocks[batch].SetVectorArray(k_Color, m_Colors[batch]);
                    Graphics.DrawMeshInstanced(
                        particleSetup.Mesh,
                        0,
                        particleSetup.Material,
                        m_Matrices[batch],
                        batchSize,
                        m_PropertyBlocks[batch]
                        /*ShadowCastingMode.Off,
                        false*/
                    );
                }
            }

            return default(JobHandle);
        }
        
        [BurstCompile]
        unsafe struct CollectRenderDataJob : IJobForEachWithEntity<LocalToWorldComponent, RenderColorComponent>
        {
            [NativeDisableUnsafePtrRestriction]
            public Matrix4x4** Matrices;
            [NativeDisableUnsafePtrRestriction]
            public Vector4** Colors;
            
            public void Execute(Entity entity, int index,
                [ReadOnly] ref LocalToWorldComponent localToWorld,
                [ReadOnly] ref RenderColorComponent color)
            {
                int batch = index / k_MaxBatchSize;
                int offset = index % k_MaxBatchSize;
                Matrices[batch][offset] = localToWorld.Value;
                Colors[batch][offset] = color.Value;
            }
        }
    }
}
