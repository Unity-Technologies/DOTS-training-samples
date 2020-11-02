using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Editor.Bridge;
using Unity.Jobs;
using Unity.Scenes;
using UnityEditor;
using UnityEngine;

namespace Unity.Entities.Editor
{
    class EntityHierarchyState : IEntityHierarchyState
    {
        static readonly string k_UnknownSceneName = L10n.Tr("<Unknown Scene>");
        static readonly string k_UnknownSubSceneName = L10n.Tr("<Unknown SubScene>");

        const int k_DefaultGenericNodeCapacity = 1024;
        const int k_DefaultEntityNodeCapacity = 1024;
        const int k_DefaultCustomNodeCapacity = 64;
        const int k_DefaultChildrenCapacity = 8;

        readonly World m_World;

        // Note: A performance issue with iterating over NativeHashMaps with medium to large capacity (regardless of the count) forces us to use Dictionaries here.
        // This prevents burstability and jobification, but it's also a 10+x speedup in the Boids sample, when there is no changes to compute.
        // We should go back to NativeHashMap if / when this performance issue is addressed.
        readonly Dictionary<EntityHierarchyNodeId, AddOperation> m_AddedNodes = new Dictionary<EntityHierarchyNodeId, AddOperation>(k_DefaultGenericNodeCapacity);
        readonly Dictionary<EntityHierarchyNodeId, MoveOperation> m_MovedNodes = new Dictionary<EntityHierarchyNodeId, MoveOperation>(k_DefaultGenericNodeCapacity);
        readonly Dictionary<EntityHierarchyNodeId, RemoveOperation> m_RemovedNodes = new Dictionary<EntityHierarchyNodeId, RemoveOperation>(k_DefaultGenericNodeCapacity);

        NativeHashMap<EntityHierarchyNodeId, Entity> m_EntityNodes = new NativeHashMap<EntityHierarchyNodeId, Entity>(k_DefaultEntityNodeCapacity, Allocator.Persistent);
        NativeHashMap<EntityHierarchyNodeId, FixedString64> m_CustomNodeNames = new NativeHashMap<EntityHierarchyNodeId, FixedString64>(k_DefaultCustomNodeCapacity, Allocator.Persistent);

        NativeHashMap<EntityHierarchyNodeId, uint> m_Versions = new NativeHashMap<EntityHierarchyNodeId, uint>(k_DefaultGenericNodeCapacity, Allocator.Persistent);
        NativeHashMap<EntityHierarchyNodeId, EntityHierarchyNodeId> m_Parents = new NativeHashMap<EntityHierarchyNodeId, EntityHierarchyNodeId>(k_DefaultGenericNodeCapacity, Allocator.Persistent);
        NativeHashMap<EntityHierarchyNodeId, UnsafeHashMap<EntityHierarchyNodeId, byte>> m_Children = new NativeHashMap<EntityHierarchyNodeId, UnsafeHashMap<EntityHierarchyNodeId, byte>>(k_DefaultGenericNodeCapacity, Allocator.Persistent);

        public EntityHierarchyState(World world)
        {
            m_World = world;
            m_Children.Add(EntityHierarchyNodeId.Root, new UnsafeHashMap<EntityHierarchyNodeId, byte>(k_DefaultChildrenCapacity, Allocator.Persistent));
            m_Versions.Add(EntityHierarchyNodeId.Root, 0);
        }

        public void Dispose()
        {
            new FreeChildrenListsJob { ChildrenLists = m_Children.GetValueArray(Allocator.TempJob) }.Run();

            m_EntityNodes.Dispose();
            m_CustomNodeNames.Dispose();

            m_Versions.Dispose();
            m_Parents.Dispose();
            m_Children.Dispose();
        }

        public bool HasChildren(in EntityHierarchyNodeId nodeId)
            => m_Children.TryGetValue(nodeId, out var childrenList) && !childrenList.IsEmpty;

        public NativeArray<EntityHierarchyNodeId> GetChildren(in EntityHierarchyNodeId nodeId, Allocator allocator)
            => m_Children[nodeId].GetKeyArray(allocator);

        public bool Exists(in EntityHierarchyNodeId nodeId)
            => m_Versions.ContainsKey(nodeId);

        public uint GetNodeVersion(in EntityHierarchyNodeId nodeId)
            => m_Versions[nodeId];

        public string GetNodeName(in EntityHierarchyNodeId nodeId)
        {
            switch (nodeId.Kind)
            {
                case NodeKind.Scene:
                {
                    var scene = SceneBridge.GetSceneByHandle(nodeId.Id);
                    return string.IsNullOrEmpty(scene.name) ? k_UnknownSceneName : scene.name;
                }
                case NodeKind.SubScene:
                {
                    // TODO: Copied from EntityHierarchyItemView.OnPingSubSceneAsset -> Move into some utility method
                    var subSceneObject = EditorUtility.InstanceIDToObject(nodeId.Id);
                    if (subSceneObject == null || !subSceneObject || !(subSceneObject is GameObject subSceneGameObject))
                        return k_UnknownSubSceneName;

                    var subScene = subSceneGameObject.GetComponent<SubScene>();
                    if (subScene == null || !subScene || subScene.SceneAsset == null || !subScene.SceneAsset)
                        return k_UnknownSubSceneName;

                    return string.IsNullOrEmpty(subScene.SceneAsset.name) ? k_UnknownSubSceneName : subScene.SceneAsset.name;
                }
                case NodeKind.Entity:
                {
                    if (!m_EntityNodes.TryGetValue(nodeId, out var entity))
                        return nodeId.ToString();

                    var name = m_World.EntityManager.GetName(entity);
                    return string.IsNullOrEmpty(name) ? entity.ToString() : name;
                }
                case NodeKind.Custom:
                {
                    return m_CustomNodeNames.TryGetValue(nodeId, out var name) ? name.ToString() : nodeId.ToString();
                }
                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        public void GetNodesBeingAdded(List<EntityHierarchyNodeId> nodesBeingAdded) => nodesBeingAdded.AddRange(m_AddedNodes.Keys);
        public void GetNodesBeingRemoved(List<EntityHierarchyNodeId> nodesBeingRemoved) => nodesBeingRemoved.AddRange(m_RemovedNodes.Keys);
        public void GetNodesBeingMoved(List<EntityHierarchyNodeId> nodesBeingMoved) => nodesBeingMoved.AddRange(m_MovedNodes.Keys);

        public bool TryGetFutureParent(in EntityHierarchyNodeId node, out EntityHierarchyNodeId nextParent)
        {
            if (m_AddedNodes.ContainsKey(node))
            {
                nextParent = m_AddedNodes[node].Parent;
                return true;
            }

            if (m_MovedNodes.ContainsKey(node))
            {
                nextParent = m_MovedNodes[node].ToNode;
                return true;
            }

            nextParent = default;
            return false;
        }

        public void RegisterAddEntityOperation(Entity entity, out EntityHierarchyNodeId generatedNode)
        {
            generatedNode = EntityHierarchyNodeId.FromEntity(entity);
            if (m_RemovedNodes.ContainsKey(generatedNode))
            {
                m_RemovedNodes.Remove(generatedNode);
            }
            else
            {
                m_AddedNodes[generatedNode] = new AddOperation
                {
                    Parent = EntityHierarchyNodeId.Root,
                    Entity = entity
                };
            }
        }

        public void RegisterAddSceneOperation(int sceneId, out EntityHierarchyNodeId generatedNode)
        {
            generatedNode = EntityHierarchyNodeId.FromScene(sceneId);
            if (m_RemovedNodes.ContainsKey(generatedNode))
                m_RemovedNodes.Remove(generatedNode);
            else
                m_AddedNodes[generatedNode] = new AddOperation { Parent = EntityHierarchyNodeId.Root };
        }

        public void RegisterAddSubSceneOperation(int subSceneId, out EntityHierarchyNodeId generatedNode)
        {
            generatedNode = EntityHierarchyNodeId.FromSubScene(subSceneId);
            if (m_RemovedNodes.ContainsKey(generatedNode))
                m_RemovedNodes.Remove(generatedNode);
            else
                m_AddedNodes[generatedNode] = new AddOperation { Parent = EntityHierarchyNodeId.Root };
        }

        public void RegisterAddCustomNodeOperation(FixedString64 name, out EntityHierarchyNodeId generatedNode)
        {
            generatedNode = EntityHierarchyNodeId.FromName(name);
            if (m_RemovedNodes.ContainsKey(generatedNode))
            {
                m_RemovedNodes.Remove(generatedNode);
            }
            else
            {
                m_AddedNodes[generatedNode] = new AddOperation
                {
                    Parent = EntityHierarchyNodeId.Root,
                    CustomName = name
                };
            }
        }

        public void RegisterRemoveOperation(in EntityHierarchyNodeId node)
        {
            if (m_AddedNodes.ContainsKey(node))
                m_AddedNodes.Remove(node);
            else
                m_RemovedNodes[node] = new RemoveOperation();
        }

        public void RegisterMoveOperation(in EntityHierarchyNodeId toNode, in EntityHierarchyNodeId node)
        {
            var previousParentNodeId = m_Parents.ContainsKey(node) ? m_Parents[node] : default;
            RegisterMoveOperationInternal(previousParentNodeId, toNode, node);
        }

        void RegisterMoveOperationInternal(in EntityHierarchyNodeId fromNode, in EntityHierarchyNodeId toNode, in EntityHierarchyNodeId node)
        {
            if (m_RemovedNodes.ContainsKey(node))
                return;

            // Move a node to root if the intended parent does not exist and will not be created in this batch
            var destinationNode = Exists(toNode) || m_AddedNodes.ContainsKey(toNode) ? toNode : EntityHierarchyNodeId.Root;

            if (m_AddedNodes.ContainsKey(node))
            {
                var addOperation = m_AddedNodes[node];
                addOperation.Parent = destinationNode;
                m_AddedNodes[node] = addOperation;
            }
            else if (m_MovedNodes.ContainsKey(node))
            {
                var moveOperation = m_MovedNodes[node];
                moveOperation.ToNode = destinationNode;
                m_MovedNodes[node] = moveOperation;
            }
            else
            {
                m_MovedNodes[node] = new MoveOperation { FromNode = fromNode, ToNode = destinationNode };
            }
        }

        public bool FlushOperations(IEntityHierarchyGroupingContext context)
        {
            // NOTE - Order matters:
            // 1.Removed -> Can cause Move operations
            // 2.Added
            // 3.Moved
            // 4.Clear operation buffers

            var hasRemovals = m_RemovedNodes.Count > 0;
            if (hasRemovals)
            {
                foreach (var node in m_RemovedNodes.Keys)
                {
                    RemoveNode(node, context.Version);

                    switch (node.Kind)
                    {
                        case NodeKind.Entity:
                        {
                            m_EntityNodes.Remove(node);
                            break;
                        }
                        case NodeKind.Custom:
                        {
                            m_CustomNodeNames.Remove(node);
                            break;
                        }
                    }
                }

                m_RemovedNodes.Clear();
            }


            var hasAdditions = m_AddedNodes.Count > 0;
            if (hasAdditions)
            {
                foreach (var kvp in m_AddedNodes)
                {
                    var node = kvp.Key;
                    var operation = kvp.Value;
                    AddNode(operation.Parent, node, context.Version);

                    switch (node.Kind)
                    {
                        case NodeKind.Entity:
                        {
                            m_EntityNodes[node] = operation.Entity;
                            break;
                        }
                        case NodeKind.Custom:
                        {
                            m_CustomNodeNames[node] = operation.CustomName;
                            break;
                        }
                    }
                }

                m_AddedNodes.Clear();
            }

            var hasMoves = false;
            if (m_MovedNodes.Count > 0)
            {
                foreach (var kvp in m_MovedNodes)
                {
                    var node = kvp.Key;
                    var operation = kvp.Value;
                    hasMoves |= MoveNode(operation.FromNode, operation.ToNode, node, context);
                }

                m_MovedNodes.Clear();
            }

            return hasAdditions || hasMoves || hasRemovals;
        }

        void AddNode(in EntityHierarchyNodeId parentNode, in EntityHierarchyNodeId newNode, uint version)
        {
            if (parentNode.Equals(default))
                throw new ArgumentException("Trying to add a new node to an invalid parent node.");

            if (newNode.Equals(default))
                throw new ArgumentException("Trying to add an invalid node to the tree.");

            m_Versions[newNode] = version;
            m_Versions[parentNode] = version;
            m_Parents[newNode] = parentNode;

            AddChild(m_Children, parentNode, newNode);
        }

        void RemoveNode(in EntityHierarchyNodeId node, uint version)
        {
            if (node.Equals(default))
                throw new ArgumentException("Trying to remove an invalid node from the tree.");

            m_Versions.Remove(node);

            if (!m_Parents.TryGetValue(node, out var parentNodeId))
                return;

            m_Parents.Remove(node);
            m_Versions[parentNodeId] = version;
            RemoveChild(m_Children, parentNodeId, node);

            // Move all children of the removed node to root
            if (m_Children.TryGetValue(node, out var children))
            {
                // Note: List might be too large for Temp allocator size limit of 16kb
                using (var childrenNodes = children.GetKeyArray(Allocator.TempJob))
                {
                    for (int i = 0, n = children.Count(); i < n; i++)
                    {
                        // Move to root if nothing else claimed that node
                        if (!m_MovedNodes.ContainsKey(childrenNodes[i]))
                            RegisterMoveOperationInternal(node, EntityHierarchyNodeId.Root, childrenNodes[i]);
                    }
                }
            }
        }

        bool MoveNode(in EntityHierarchyNodeId previousParent, in EntityHierarchyNodeId newParent, in EntityHierarchyNodeId node, IEntityHierarchyGroupingContext context)
        {
            if (previousParent.Equals(default))
                throw new ArgumentException("Trying to unparent from an invalid node.");

            if (newParent.Equals(default))
                throw new ArgumentException("Trying to parent to an invalid node.");

            if (node.Equals(default))
                throw new ArgumentException("Trying to add an invalid node to the tree.");

            if (previousParent.Equals(newParent))
                return false; // NOOP

            if (m_Parents[node] == newParent)
                return false; // NOOP

            RemoveChild(m_Children, previousParent, node);
            if (Exists(previousParent))
                m_Versions[previousParent] = context.Version;

            m_Parents[node] = newParent;
            AddChild(m_Children, newParent, node);
            m_Versions[newParent] = context.Version;
            return true;
        }

        static void AddChild(NativeHashMap<EntityHierarchyNodeId, UnsafeHashMap<EntityHierarchyNodeId, byte>> children, in EntityHierarchyNodeId parentId, in EntityHierarchyNodeId newChild)
        {
            if (!children.TryGetValue(parentId, out var siblings))
                siblings = new UnsafeHashMap<EntityHierarchyNodeId, byte>(k_DefaultChildrenCapacity, Allocator.Persistent);

            siblings.Add(newChild, 0);
            children[parentId] = siblings;
        }

        static void RemoveChild(NativeHashMap<EntityHierarchyNodeId, UnsafeHashMap<EntityHierarchyNodeId, byte>> children, in EntityHierarchyNodeId parentId, in EntityHierarchyNodeId childToRemove)
        {
            if (!children.TryGetValue(parentId, out var siblings))
                return;

            siblings.Remove(childToRemove);
            children[parentId] = siblings;
        }

        struct AddOperation
        {
            public EntityHierarchyNodeId Parent;

            // Possible Payloads
            public Entity Entity;
            public FixedString64 CustomName;
        }

        struct MoveOperation
        {
            public EntityHierarchyNodeId FromNode;
            public EntityHierarchyNodeId ToNode;
        }

        struct RemoveOperation
        {
        }

        [BurstCompile]
        struct FreeChildrenListsJob : IJob
        {
            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<UnsafeHashMap<EntityHierarchyNodeId, byte>> ChildrenLists;

            public void Execute()
            {
                for (var i = 0; i < ChildrenLists.Length; i++)
                {
                    ChildrenLists[i].Dispose();
                }
            }
        }
    }
}
