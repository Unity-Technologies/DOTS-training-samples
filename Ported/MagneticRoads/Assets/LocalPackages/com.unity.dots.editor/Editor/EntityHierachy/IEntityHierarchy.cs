using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Entities.Editor
{
    interface IEntityHierarchy
    {
        IEntityHierarchyState State { get; }
        IEntityHierarchyGroupingStrategy GroupingStrategy { get; }
        EntityQueryDesc QueryDesc { get; }
        World World { get; }
    }

    interface IEntityHierarchyState : IDisposable
    {
        bool Exists(in EntityHierarchyNodeId nodeId);
        bool HasChildren(in EntityHierarchyNodeId nodeId);
        NativeArray<EntityHierarchyNodeId> GetChildren(in EntityHierarchyNodeId nodeId, Allocator allocator);
        uint GetNodeVersion(in EntityHierarchyNodeId nodeId);

        // TODO More explicit Name (this is an expensive operation): ComputeNodeName / RetrieveNodeName / FetchNodeName / BuildNodeName / etc.
        string GetNodeName(in EntityHierarchyNodeId nodeId);

        void GetNodesBeingAdded(List<EntityHierarchyNodeId> nodesBeingAdded);
        void GetNodesBeingRemoved(List<EntityHierarchyNodeId> nodesBeingRemoved);
        void GetNodesBeingMoved(List<EntityHierarchyNodeId> nodesBeingMoved);
        bool TryGetFutureParent(in EntityHierarchyNodeId node, out EntityHierarchyNodeId nextParent);

        // TODO More explicit Name: EnqueueOperation? StageOperation?
        void RegisterAddEntityOperation(Entity entity, out EntityHierarchyNodeId generatedNode);
        void RegisterAddSceneOperation(int sceneId, out EntityHierarchyNodeId generatedNode);
        void RegisterAddSubSceneOperation(int subSceneId, out EntityHierarchyNodeId generatedNode);
        void RegisterAddCustomNodeOperation(FixedString64 name, out EntityHierarchyNodeId generatedNode);
        void RegisterRemoveOperation(in EntityHierarchyNodeId node);
        void RegisterMoveOperation(in EntityHierarchyNodeId toNode, in EntityHierarchyNodeId node);

        // TODO More explicit Name: Commit? Push? Submit? ApplyAndClear?
        bool FlushOperations(IEntityHierarchyGroupingContext context);
    }

    interface IEntityHierarchyGroupingStrategy : IDisposable
    {
        ComponentType[] ComponentsToWatch { get; }

        void BeginApply(IEntityHierarchyGroupingContext context);
        void ApplyEntityChanges(NativeArray<Entity> newEntities, NativeArray<Entity> removedEntities, IEntityHierarchyGroupingContext context);
        void ApplyComponentDataChanges(ComponentType componentType, in ComponentDataDiffer.ComponentChanges componentChanges, IEntityHierarchyGroupingContext context);
        void ApplySharedComponentDataChanges(ComponentType componentType, in SharedComponentDataDiffer.ComponentChanges componentChanges, IEntityHierarchyGroupingContext context);
        bool EndApply(IEntityHierarchyGroupingContext context);
    }

    interface IEntityHierarchyGroupingContext
    {
        uint Version { get; }
        ISceneMapper SceneMapper { get; }
    }
}
