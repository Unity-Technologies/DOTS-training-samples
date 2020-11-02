using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Transforms;

namespace Unity.Entities.Editor
{
    class EntityHierarchyDefaultGroupingStrategy : IEntityHierarchyGroupingStrategy
    {
        const int k_DefaultEntityNodeCapacity = 1024;
        const int k_DefaultSceneNodeCapacity = 64;

        readonly int m_ChildTypeIndex;
        readonly World m_World;
        readonly IEntityHierarchyState m_State;

        // TODO: Replace with NativeHashSet when available + need to burst this
        readonly HashSet<Entity> m_KnownMissingParent = new HashSet<Entity>();

        NativeHashMap<Entity, SceneTag> m_SceneTagPerEntity = new NativeHashMap<Entity, SceneTag>(k_DefaultEntityNodeCapacity, Allocator.Persistent);
        NativeHashMap<EntityHierarchyNodeId, Hash128> m_SceneNodes = new NativeHashMap<EntityHierarchyNodeId, Hash128>(k_DefaultSceneNodeCapacity, Allocator.Persistent);

        EntityQuery m_RootEntitiesQuery;
        EntityQueryMask m_RootEntitiesQueryMask;

        EntityQuery m_ParentEntitiesQuery;
        EntityQueryMask m_ParentEntitiesQueryMask;

        public EntityHierarchyDefaultGroupingStrategy(World world, IEntityHierarchyState state)
        {
            m_World = world;
            m_State = state;

            m_RootEntitiesQuery = m_World.EntityManager.CreateEntityQuery(new EntityQueryDesc { None = new ComponentType[] { typeof(Parent) } });
            m_RootEntitiesQueryMask = m_World.EntityManager.GetEntityQueryMask(m_RootEntitiesQuery);

            m_ChildTypeIndex = TypeManager.GetTypeIndex(typeof(Child));
            m_ParentEntitiesQuery = m_World.EntityManager.CreateEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Child) } });
            m_ParentEntitiesQueryMask = m_World.EntityManager.GetEntityQueryMask(m_ParentEntitiesQuery);
        }

        public void Dispose()
        {
            if (m_World.IsCreated)
            {
                if (m_World.EntityManager.IsQueryValid(m_RootEntitiesQuery))
                    m_RootEntitiesQuery.Dispose();

                if (m_World.EntityManager.IsQueryValid(m_ParentEntitiesQuery))
                    m_ParentEntitiesQuery.Dispose();
            }

            m_SceneTagPerEntity.Dispose();
            m_SceneNodes.Dispose();
        }

        public ComponentType[] ComponentsToWatch { get; } = { typeof(Parent), typeof(SceneTag) };

        void IEntityHierarchyGroupingStrategy.BeginApply(IEntityHierarchyGroupingContext context)
        {
        }

        void IEntityHierarchyGroupingStrategy.ApplyEntityChanges(NativeArray<Entity> newEntities, NativeArray<Entity> removedEntities, IEntityHierarchyGroupingContext context)
        {
            // Remove entities
            foreach (var entity in removedEntities)
                m_State.RegisterRemoveOperation(EntityHierarchyNodeId.FromEntity(entity));

            // Add new entities
            foreach (var entity in newEntities)
                m_State.RegisterAddEntityOperation(entity, out _);

            UpdateMissingParentEntities();
            MoveEntitiesUnderFoundMissingParents();
        }

        void IEntityHierarchyGroupingStrategy.ApplyComponentDataChanges(ComponentType componentType, in ComponentDataDiffer.ComponentChanges componentChanges, IEntityHierarchyGroupingContext context)
        {
            if (componentType == typeof(Parent))
                ApplyParentComponentChanges(componentChanges);
        }

        void IEntityHierarchyGroupingStrategy.ApplySharedComponentDataChanges(ComponentType componentType, in SharedComponentDataDiffer.ComponentChanges componentChanges, IEntityHierarchyGroupingContext context)
        {
            if (componentType == typeof(SceneTag))
                ApplySceneTagChanges(componentChanges, context);
        }

        bool IEntityHierarchyGroupingStrategy.EndApply(IEntityHierarchyGroupingContext context)
        {
            FixSceneParenting(context);

            var hasChanges = m_State.FlushOperations(context);

            if (hasChanges)
                RemoveEmptySceneNodes(context);

            return hasChanges;
        }

        void FixSceneParenting(IEntityHierarchyGroupingContext context)
        {
            // Find entity nodes that became orphaned and parent them to the proper subscene, if it exists.
            using (var nodesBeingRemoved = PooledList<EntityHierarchyNodeId>.Make())
            using (var nodesBeingMoved = PooledList<EntityHierarchyNodeId>.Make())
            {
                m_State.GetNodesBeingRemoved(nodesBeingRemoved.List);
                m_State.GetNodesBeingMoved(nodesBeingMoved.List);

                foreach (var node in nodesBeingRemoved.List)
                {
                    if (m_State.HasChildren(node) && node.Kind == NodeKind.Entity)
                    {
                        var subsceneHash = context.SceneMapper.GetSubsceneHash(m_World, node.ToEntity());
                        var newParent = subsceneHash == default ? EntityHierarchyNodeId.Root : GetOrCreateSubsceneNode(subsceneHash, context);

                        foreach (var orphan in m_State.GetChildren(in node, Allocator.Temp))
                        {
                            if (!nodesBeingMoved.List.Contains(orphan))
                            {
                                // Orphan detected, ensure it gets parented to the correct subscene, if any
                                m_State.RegisterMoveOperation(newParent, orphan);
                            }
                        }
                    }
                }
            }
        }

        void RemoveEmptySceneNodes(IEntityHierarchyGroupingContext context)
        {
            bool TryRemove(NodeKind nodeKind)
            {
                var removalsPerformed = false;
                var sceneNodes = m_SceneNodes.GetKeyArray(Allocator.Temp);
                for (var i = 0; i < sceneNodes.Length; i++)
                {
                    var sceneNode = sceneNodes[i];
                    if (sceneNode.Kind != nodeKind || m_State.HasChildren(sceneNode))
                        continue;

                    m_SceneNodes.Remove(sceneNode);
                    m_State.RegisterRemoveOperation(sceneNode);
                    removalsPerformed = true;
                }

                sceneNodes.Dispose();
                return removalsPerformed;
            }

            if (TryRemove(NodeKind.SubScene))
                m_State.FlushOperations(context);
            if (TryRemove(NodeKind.Scene))
                m_State.FlushOperations(context);
        }

         void UpdateMissingParentEntities()
         {
             using (var nodesBeingRemoved = PooledList<EntityHierarchyNodeId>.Make())
             {
                 m_State.GetNodesBeingRemoved(nodesBeingRemoved.List);
                 if (nodesBeingRemoved.List.Count == 0)
                     return;

                 // Get a native array of entities to actually remove
                 var filteredRemovedEntities = new NativeList<Entity>(nodesBeingRemoved.List.Count, Allocator.TempJob);
                 foreach (var removedNode in nodesBeingRemoved.List)
                 {
                     if (removedNode.TryConvertToEntity(out var entity))
                         filteredRemovedEntities.Add(entity);
                 }

                 // Filter only the entities with a Child component
                 var removedParentEntities = new NativeList<Entity>(Allocator.TempJob);
                 new FilterEntitiesWithQueryMask
                 {
                     QueryMask = m_ParentEntitiesQueryMask,
                     Source = filteredRemovedEntities,
                     Result = removedParentEntities
                 }.Run();

                 filteredRemovedEntities.Dispose();

                 // Aggregate with all known missing parent cache
                 for (var i = 0; i < removedParentEntities.Length; i++)
                 {
                     m_KnownMissingParent.Add(removedParentEntities[i]);
                 }

                 removedParentEntities.Dispose();
             }
         }

         unsafe void MoveEntitiesUnderFoundMissingParents()
         {
             using (var nodesBeingAdded = PooledList<EntityHierarchyNodeId>.Make())
             {
                 m_State.GetNodesBeingAdded(nodesBeingAdded.List);
                 if (nodesBeingAdded.List.Count == 0)
                     return;

                 // Find all missing parent in this changeset
                 var missingParentDetectedInThisBatch = new NativeList<Entity>(Allocator.TempJob);
                 foreach (var addedNode in nodesBeingAdded.List)
                 {
                     if (addedNode.TryConvertToEntity(out var entity))
                     {
                         if (!m_KnownMissingParent.Remove(entity))
                             continue;

                         missingParentDetectedInThisBatch.Add(entity);
                     }
                 }

                 // Find all children for each newly found missing parent
                 if (missingParentDetectedInThisBatch.Length > 0)
                 {
                     var childrenPerParent = new NativeArray<UnsafeList<Entity>>(missingParentDetectedInThisBatch.Length, Allocator.TempJob);
                     var bufferAccessor = m_World.EntityManager.GetBufferFromEntity<Child>(true);

                     new FindAllChildrenOfEntity
                         {
                             EntityComponentStore = m_World.EntityManager.GetCheckedEntityDataAccess()->EntityComponentStore,
                             ChildTypeIndex = m_ChildTypeIndex,
                             BufferAccessor = bufferAccessor,
                             ChildrenPerParent = childrenPerParent,
                             NewFoundParent = missingParentDetectedInThisBatch
                         }.Schedule(missingParentDetectedInThisBatch.Length, 1)
                          .Complete();

                     // Remap children to formerly missing parent
                     for (var i = 0; i < missingParentDetectedInThisBatch.Length; i++)
                     {
                         var children = childrenPerParent[i];
                         if (!children.IsCreated)
                             continue;

                         var parent = EntityHierarchyNodeId.FromEntity(missingParentDetectedInThisBatch[i]);
                         for (var j = 0; j < children.Length; j++)
                         {
                             var child = children[j];
                             var childNodeId = EntityHierarchyNodeId.FromEntity(child);
                             if (m_State.Exists(childNodeId))
                             {
                                 m_State.RegisterMoveOperation(parent, childNodeId);
                             }
                         }

                         children.Dispose();
                     }

                     childrenPerParent.Dispose();
                 }

                 missingParentDetectedInThisBatch.Dispose();
             }
         }

        void ApplyParentComponentChanges(ComponentDataDiffer.ComponentChanges componentChanges)
        {
            // parent removed
            if (componentChanges.RemovedComponentsCount > 0)
            {
                var (entities, parents) = componentChanges.GetRemovedComponents<Parent>(Allocator.TempJob);
                for (var i = 0; i < componentChanges.RemovedComponentsCount; i++)
                {
                    var entityNodeId = EntityHierarchyNodeId.FromEntity(entities[i]);
                    m_State.RegisterMoveOperation(EntityHierarchyNodeId.Root, entityNodeId);
                }

                entities.Dispose();
                parents.Dispose();
            }

            // parent added
            if (componentChanges.AddedComponentsCount > 0)
            {
                using (var nodesBeingAdded = PooledList<EntityHierarchyNodeId>.Make())
                {
                    m_State.GetNodesBeingAdded(nodesBeingAdded.List);
                    var (entities, parents) = componentChanges.GetAddedComponents<Parent>(Allocator.TempJob);
                    for (var i = 0; i < componentChanges.AddedComponentsCount; i++)
                    {
                        var entity = entities[i];
                        var entityNodeId = EntityHierarchyNodeId.FromEntity(entity);
                        var newParentComponent = parents[i];
                        var newParentEntity = newParentComponent.Value;
                        var newParentEntityNodeId = EntityHierarchyNodeId.FromEntity(newParentEntity);

                        if (!m_State.Exists(newParentEntityNodeId) && !nodesBeingAdded.List.Contains(newParentEntityNodeId))
                        {
                            m_KnownMissingParent.Add(newParentEntity);
                            m_State.RegisterMoveOperation(EntityHierarchyNodeId.Root, entityNodeId);
                        }
                        else
                        {
                            m_State.RegisterMoveOperation(newParentEntityNodeId, entityNodeId);
                        }
                    }

                    entities.Dispose();
                    parents.Dispose();
                }
            }
        }

        void ApplySceneTagChanges(SharedComponentDataDiffer.ComponentChanges componentChanges, IEntityHierarchyGroupingContext context)
        {
            for (var i = 0; i < componentChanges.RemovedEntitiesCount; ++i)
            {
                var entity = componentChanges.GetRemovedEntity(i);
                m_SceneTagPerEntity.Remove(entity);
            }

            using (var nodesBeingAdded = PooledList<EntityHierarchyNodeId>.Make())
            {
                m_State.GetNodesBeingAdded(nodesBeingAdded.List);
                for (var i = 0; i < componentChanges.AddedEntitiesCount; ++i)
                {
                    var entity = componentChanges.GetAddedEntity(i);
                    var entityNodeId = EntityHierarchyNodeId.FromEntity(entity);
                    var tag = componentChanges.GetAddedComponent<SceneTag>(i);
                    m_SceneTagPerEntity[entity] = tag;

                    if (m_RootEntitiesQueryMask.Matches(entity)
                     || nodesBeingAdded.List.Contains(entityNodeId)
                     && m_State.TryGetFutureParent(entityNodeId, out var nextParent)
                     && nextParent == EntityHierarchyNodeId.Root)
                    {
                        var subsceneHash = context.SceneMapper.GetSubsceneHash(m_World, tag.SceneEntity);
                        var newParentNodeId = subsceneHash == default ? EntityHierarchyNodeId.Root : GetOrCreateSubsceneNode(subsceneHash, context);
                        m_State.RegisterMoveOperation(newParentNodeId, entityNodeId);
                    }
                }
            }
        }

        EntityHierarchyNodeId GetOrCreateSubsceneNode(Hash128 subSceneHash, IEntityHierarchyGroupingContext context)
        {
            if (!context.SceneMapper.TryGetSceneOrSubSceneInstanceId(subSceneHash, out var subSceneInstanceId))
            {
                Debug.LogWarning($"SubScene hash {subSceneHash} not found in {nameof(SceneMapper)} state, unable to create node id for it.");
                return default;
            }

            var subSceneNodeId = EntityHierarchyNodeId.FromSubScene(subSceneInstanceId);
            if (!m_State.Exists(subSceneNodeId))
            {
                var parentSceneHash = context.SceneMapper.GetParentSceneHash(subSceneHash);
                if (!context.SceneMapper.TryGetSceneOrSubSceneInstanceId(parentSceneHash, out var parentSceneInstanceId))
                {
                    Debug.LogWarning($"Scene hash {parentSceneHash} not found in {nameof(SceneMapper)} state, unable to create node id for it.");
                    return default;
                }

                var parentSceneNodeId = EntityHierarchyNodeId.FromScene(parentSceneInstanceId);
                if (!m_State.Exists(parentSceneNodeId))
                {
                    m_SceneNodes[parentSceneNodeId] = parentSceneHash;

                    m_State.RegisterAddSceneOperation(parentSceneInstanceId, out parentSceneNodeId);
                    m_State.RegisterMoveOperation(EntityHierarchyNodeId.Root, parentSceneNodeId);
                }

                m_SceneNodes[subSceneNodeId] = subSceneHash;

                m_State.RegisterAddSubSceneOperation(subSceneInstanceId, out subSceneNodeId);
                m_State.RegisterMoveOperation(parentSceneNodeId, subSceneNodeId);
            }

            return subSceneNodeId;
        }

        [BurstCompile]
        struct FilterEntitiesWithQueryMask : IJob
        {
            [ReadOnly] public EntityQueryMask QueryMask;
            [ReadOnly] public NativeArray<Entity> Source;
            [WriteOnly] public NativeList<Entity> Result;

            public void Execute()
            {
                for (var i = 0; i < Source.Length; i++)
                {
                    var entity = Source[i];
                    if (QueryMask.Matches(entity))
                        Result.Add(entity);
                }
            }
        }

        [BurstCompile]
        unsafe struct FindAllChildrenOfEntity : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction] public EntityComponentStore* EntityComponentStore;
            public int ChildTypeIndex;

            [ReadOnly] public BufferFromEntity<Child> BufferAccessor;
            [ReadOnly] public NativeArray<Entity> NewFoundParent;
            [WriteOnly] public NativeArray<UnsafeList<Entity>> ChildrenPerParent;

            public void Execute(int index)
            {
                var entity = NewFoundParent[index];
                if (EntityComponentStore->HasComponent(entity, ChildTypeIndex))
                {
                    var b = BufferAccessor[entity];
                    var children = new UnsafeList<Entity>(b.Length, Allocator.TempJob);
                    for (var i = 0; i < b.Length; i++)
                    {
                        children.Add(b[i].Value);
                    }

                    ChildrenPerParent[index] = children;
                }
            }
        }
    }
}
