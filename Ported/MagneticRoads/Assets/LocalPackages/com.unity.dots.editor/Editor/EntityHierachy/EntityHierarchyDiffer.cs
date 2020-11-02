using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;

namespace Unity.Entities.Editor
{
    class EntityHierarchyDiffer : IEntityHierarchyGroupingContext, IDisposable
    {
        static readonly ProfilerMarker k_TryUpdateMarker = new ProfilerMarker($"{nameof(EntityHierarchyDiffer)}.{nameof(TryUpdate)}");
        static readonly ProfilerMarker k_TryUpdateCompleteJobs = new ProfilerMarker($"{nameof(EntityHierarchyDiffer)}.{nameof(TryUpdate)} complete jobs");
        static readonly ProfilerMarker k_GetDiffSinceLastFrameAsync = new ProfilerMarker($"{nameof(EntityHierarchyDiffer)}.{nameof(GetDiffSinceLastFrameAsync)}");
        static readonly ProfilerMarker k_ApplyDiffResultsToStrategy = new ProfilerMarker($"{nameof(EntityHierarchyDiffer)}.{nameof(ApplyDiffResultsToStrategy)}");
        static readonly ProfilerMarker k_UpdateCachedQueries = new ProfilerMarker($"{nameof(EntityHierarchyDiffer)}.{nameof(UpdateCachedQueries)}");


        readonly IEntityHierarchy m_Hierarchy;
        readonly EntityDiffer m_EntityDiffer;
        readonly Cooldown m_Cooldown;
        readonly SceneMapper m_SceneMapper;

        readonly List<ComponentDataDiffer> m_ComponentDataDiffers = new List<ComponentDataDiffer>();
        readonly List<SharedComponentDataDiffer> m_SharedComponentDataDiffers = new List<SharedComponentDataDiffer>();

        EntityQueryDesc m_CachedQueryDescription;
        EntityQuery m_MainQuery;

        // Storage for temp differ results
        NativeList<Entity> m_NewEntities;
        NativeList<Entity> m_RemovedEntities;
        readonly ComponentDataDiffer.ComponentChanges[] m_ComponentDataDifferResults;
        readonly SharedComponentDataDiffer.ComponentChanges[] m_SharedComponentDataDifferResults;
        NativeArray<JobHandle> m_DifferHandles;

        public uint Version => m_Hierarchy.World.EntityManager.GlobalSystemVersion;

        ISceneMapper IEntityHierarchyGroupingContext.SceneMapper => m_SceneMapper;

        public EntityHierarchyDiffer(IEntityHierarchy hierarchy, int cooldownDurationInMs = 0)
        {
            m_Hierarchy = hierarchy;
            m_Cooldown = new Cooldown(TimeSpan.FromMilliseconds(cooldownDurationInMs));
            m_SceneMapper = new SceneMapper();

            foreach (var componentType in hierarchy.GroupingStrategy.ComponentsToWatch)
            {
                if (!ComponentDataDiffer.CanWatch(componentType) && !SharedComponentDataDiffer.CanWatch(componentType))
                    throw new NotSupportedException($" The component {componentType} requested by strategy of type {hierarchy.GroupingStrategy.GetType()} cannot be watched. No suitable differ available.");
            }

            m_Hierarchy = hierarchy;
            m_EntityDiffer = new EntityDiffer(hierarchy.World);
            foreach (var componentToWatch in hierarchy.GroupingStrategy.ComponentsToWatch)
            {
                var typeInfo = TypeManager.GetTypeInfo(componentToWatch.TypeIndex);

                switch (typeInfo.Category)
                {
                    case TypeManager.TypeCategory.ComponentData when UnsafeUtility.IsUnmanaged(componentToWatch.GetManagedType()):
                        m_ComponentDataDiffers.Add((new ComponentDataDiffer(componentToWatch)));
                        break;
                    case TypeManager.TypeCategory.ISharedComponentData:
                        m_SharedComponentDataDiffers.Add((new SharedComponentDataDiffer(componentToWatch)));
                        break;
                }
            }

            m_ComponentDataDifferResults = new ComponentDataDiffer.ComponentChanges[m_ComponentDataDiffers.Count];
            m_SharedComponentDataDifferResults = new SharedComponentDataDiffer.ComponentChanges[m_SharedComponentDataDiffers.Count];

            m_NewEntities = new NativeList<Entity>(1024, Allocator.Persistent);
            m_RemovedEntities = new NativeList<Entity>(1024, Allocator.Persistent);
            m_DifferHandles = new NativeArray<JobHandle>(m_ComponentDataDiffers.Count + 1, Allocator.Persistent);
        }

        public void Dispose()
        {
            m_SceneMapper.Dispose();

            m_EntityDiffer.Dispose();
            if (m_MainQuery != default && m_Hierarchy.World != null && m_Hierarchy.World.IsCreated && m_MainQuery != m_Hierarchy.World.EntityManager.UniversalQuery && m_Hierarchy.World.EntityManager.IsQueryValid(m_MainQuery))
                m_MainQuery.Dispose();

            foreach (var componentDataDiffer in m_ComponentDataDiffers)
                componentDataDiffer.Dispose();

            foreach (var sharedComponentDataDiffer in m_SharedComponentDataDiffers)
                sharedComponentDataDiffer.Dispose();

            m_NewEntities.Dispose();
            m_RemovedEntities.Dispose();
            m_DifferHandles.Dispose();
        }

        public bool TryUpdate(out bool structuralChangeDetected)
        {
            if (!m_Cooldown.Update(DateTime.Now))
            {
                structuralChangeDetected = false;
                return false;
            }

            using (k_TryUpdateMarker.Auto())
            {
                var handle = GetDiffSinceLastFrameAsync();

                var sceneManagerDirty = m_SceneMapper.SceneManagerDirty;
                try
                {
                    try
                    {
                        m_SceneMapper.Update(); // throws
                    }
                    finally
                    {
                        using (k_TryUpdateCompleteJobs.Auto())
                        {
                            handle.Complete();
                        }
                    }

                    var strategyStateChanged = ApplyDiffResultsToStrategy();
                    structuralChangeDetected = sceneManagerDirty || strategyStateChanged;
                }
                finally
                {
                    DisposeDifferResults();
                }

                return true;
            }
        }

        void DisposeDifferResults()
        {
            foreach (var r in m_ComponentDataDifferResults)
                r.Dispose();

            foreach (var r in m_SharedComponentDataDifferResults)
                r.Dispose();
        }

        JobHandle GetDiffSinceLastFrameAsync()
        {
            if (!m_Hierarchy.World.IsCreated)
            {
                // Diffing utility is still called by the window but the world has been destroyed
                // The window will unregister this as soon as switching world will be done
                return default;
            }

            using (k_GetDiffSinceLastFrameAsync.Auto())
            {
                UpdateCachedQueries();

                var handleIdx = 0;
                m_DifferHandles[handleIdx++] = m_EntityDiffer.GetEntityQueryMatchDiffAsync(m_MainQuery, m_NewEntities, m_RemovedEntities);

                for (var i = 0; i < m_ComponentDataDiffers.Count; i++)
                {
                    m_ComponentDataDifferResults[i] = m_ComponentDataDiffers[i].GatherComponentChangesAsync(m_MainQuery, Allocator.TempJob, out var componentDataDifferHandle);
                    m_DifferHandles[handleIdx++] = componentDataDifferHandle;
                }

                for (var i = 0; i < m_SharedComponentDataDiffers.Count; i++)
                {
                    m_SharedComponentDataDifferResults[i] = m_SharedComponentDataDiffers[i].GatherComponentChanges(m_Hierarchy.World.EntityManager, m_MainQuery, Allocator.TempJob);
                }

                var handle = JobHandle.CombineDependencies(m_DifferHandles);

                return handle;
            }
        }

        void UpdateCachedQueries()
        {
            using (k_UpdateCachedQueries.Auto())
            {
                var entityManager = m_Hierarchy.World.EntityManager;

                if (m_Hierarchy.QueryDesc != null && m_Hierarchy.QueryDesc == m_CachedQueryDescription
                    || m_Hierarchy.QueryDesc == null && m_MainQuery == entityManager.UniversalQuery)
                    return;

                m_CachedQueryDescription = m_Hierarchy.QueryDesc;
                if (m_MainQuery != entityManager.UniversalQuery && entityManager.IsQueryValid(m_MainQuery))
                    m_MainQuery.Dispose();

                if (m_Hierarchy.QueryDesc != null)
                {
                    try
                    {
                        m_MainQuery = entityManager.CreateEntityQuery(m_Hierarchy.QueryDesc);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(new Exception($"Entities window: Unable to filter entities, query contains too many {m_Hierarchy.QueryDesc.Any.Length} different types.", e));
                        m_MainQuery = entityManager.UniversalQuery;
                    }
                }
                else
                    m_MainQuery = entityManager.UniversalQuery;
            }
        }

        bool ApplyDiffResultsToStrategy()
        {
            if (!m_Hierarchy.World.IsCreated)
            {
                // Diffing utility is still called by the window but the world has been destroyed
                // The window will unregister this as soon as switching world will be done
                return default;
            }

            using (k_ApplyDiffResultsToStrategy.Auto())
            {
                var strategy = m_Hierarchy.GroupingStrategy;
                strategy.BeginApply(this);
                strategy.ApplyEntityChanges(m_NewEntities, m_RemovedEntities, this);

                for (var i = 0; i < m_ComponentDataDifferResults.Length; i++)
                {
                    var componentType = m_ComponentDataDiffers[i].WatchedComponentType;
                    strategy.ApplyComponentDataChanges(componentType, m_ComponentDataDifferResults[i], this);
                }

                for (var i = 0; i < m_SharedComponentDataDifferResults.Length; i++)
                {
                    var componentType = m_SharedComponentDataDiffers[i].WatchedComponentType;
                    strategy.ApplySharedComponentDataChanges(componentType, m_SharedComponentDataDifferResults[i], this);
                }

                var strategyStateChanged = strategy.EndApply(this);

                return strategyStateChanged;
            }
        }
    }
}



