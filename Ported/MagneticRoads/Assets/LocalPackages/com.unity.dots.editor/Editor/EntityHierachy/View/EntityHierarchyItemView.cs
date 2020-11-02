using System;
using Unity.Scenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class EntityHierarchyItemView : VisualElement, IPoolable
    {
        static readonly string k_PingSubSceneInHierarchy = L10n.Tr("Ping sub scene in hierarchy");
        static readonly string k_PingSubSceneInProjectWindow = L10n.Tr("Ping sub scene in project window");

        public VisualElement Owner { get; set; }

        readonly VisualElement m_Icon;
        readonly Label m_NameLabel;
        readonly VisualElement m_SystemButton;
        readonly VisualElement m_PingGameObject;

        EntityHierarchyItem m_Item;
        int? m_OriginatingId;
        IManipulator m_ContextMenuManipulator;

        public EntityHierarchyItemView()
        {
            Resources.Templates.EntityHierarchyItem.Clone(this);
            AddToClassList(UssClasses.DotsEditorCommon.CommonResources);
            AddToClassList(UssClasses.Resources.EntityHierarchy);

            m_Icon = this.Q<VisualElement>(className: UssClasses.EntityHierarchyWindow.Item.Icon);
            m_NameLabel = this.Q<Label>(className: UssClasses.EntityHierarchyWindow.Item.NameLabel);
            m_SystemButton = this.Q<VisualElement>(className: UssClasses.EntityHierarchyWindow.Item.SystemButton);
            m_PingGameObject = this.Q<VisualElement>(className: UssClasses.EntityHierarchyWindow.Item.PingGameObjectButton);
        }

        public void SetSource(EntityHierarchyItem item)
        {
            // Needs to be cleared here because list virtualization doesn't use IPoolable.Reset()
            ClearDynamicStyles();

            m_Item = item;
            switch (m_Item.NodeId.Kind)
            {
                case NodeKind.Entity:
                {
                    RenderEntityNode();
                    break;
                }
                case NodeKind.Scene:
                case NodeKind.SubScene:
                {
                    RenderSceneNode(m_Item.NodeId.Kind == NodeKind.SubScene);
                    break;
                }
                case NodeKind.Custom:
                {
                    RenderCustomNode();
                    break;
                }
                case NodeKind.Root:
                case NodeKind.None:
                {
                    RenderInvalidNode(m_Item.NodeId);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IPoolable.Reset()
        {
            Owner = null;
            if (m_ContextMenuManipulator != null)
            {
                this.RemoveManipulator(m_ContextMenuManipulator);
                UnregisterCallback<ContextualMenuPopulateEvent>(OnSceneContextMenu);
                m_ContextMenuManipulator = null;
            }

            m_Item = null;
            m_OriginatingId = null;
        }

        void IPoolable.ReturnToPool() => EntityHierarchyPool.ReturnVisualElement(this);

        void RenderEntityNode()
        {
            m_NameLabel.text = m_Item.CachedName;

            m_Icon.AddToClassList(UssClasses.EntityHierarchyWindow.Item.IconEntity);

            // TODO: Re-enable once we have connectivity between DOTS Editor Tools
            // m_SystemButton.AddToClassList(UssClasses.EntityHierarchyWindow.Item.VisibleOnHover);

            if (TryGetSourceGameObjectId(m_Item.NodeId.ToEntity(), m_Item.World, out var originatingId))
            {
                m_OriginatingId = originatingId;
                m_PingGameObject.AddToClassList(UssClasses.EntityHierarchyWindow.Item.VisibleOnHover);
                m_PingGameObject.RegisterCallback<MouseUpEvent>(OnPingGameObjectRequested);
            }
        }

        void OnPingGameObjectRequested(MouseUpEvent _)
        {
            if (!m_OriginatingId.HasValue)
                return;

            EditorGUIUtility.PingObject(m_OriginatingId.Value);
        }

        void RenderSceneNode(bool isSubScene)
        {
            AddToClassList(UssClasses.EntityHierarchyWindow.Item.SceneNode);
            m_Icon.AddToClassList(UssClasses.EntityHierarchyWindow.Item.IconScene);
            m_NameLabel.AddToClassList(UssClasses.EntityHierarchyWindow.Item.NameScene);
            m_NameLabel.text = m_Item.CachedName;

            if (isSubScene)
            {
                m_ContextMenuManipulator = new ContextualMenuManipulator(null);
                this.AddManipulator(m_ContextMenuManipulator);
                RegisterCallback<ContextualMenuPopulateEvent>(OnSceneContextMenu);
            }
        }

        void RenderCustomNode()
        {
            // TODO: Eventually, add a generic icon and a style that are overridable
            m_NameLabel.text = m_Item.CachedName;
        }

        void RenderInvalidNode(EntityHierarchyNodeId nodeId)
        {
            m_NameLabel.text = $"<UNKNOWN> ({nodeId.ToString()})";
        }

        void ClearDynamicStyles()
        {
            RemoveFromClassList(UssClasses.EntityHierarchyWindow.Item.SceneNode);

            m_NameLabel.RemoveFromClassList(UssClasses.EntityHierarchyWindow.Item.NameScene);

            m_Icon.RemoveFromClassList(UssClasses.EntityHierarchyWindow.Item.IconScene);
            m_Icon.RemoveFromClassList(UssClasses.EntityHierarchyWindow.Item.IconEntity);

            m_SystemButton.RemoveFromClassList(UssClasses.EntityHierarchyWindow.Item.VisibleOnHover);
            m_PingGameObject.RemoveFromClassList(UssClasses.EntityHierarchyWindow.Item.VisibleOnHover);

            m_PingGameObject.UnregisterCallback<MouseUpEvent>(OnPingGameObjectRequested);
        }

        static bool TryGetSourceGameObjectId(Entity entity, World world, out int? originatingId)
        {
            if (!world.EntityManager.Exists(entity) || !world.EntityManager.HasComponent<EntityGuid>(entity))
            {
                originatingId = null;
                return false;
            }

            originatingId = world.EntityManager.GetComponentData<EntityGuid>(entity).OriginatingId;
            return true;
        }

        void OnSceneContextMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction(k_PingSubSceneInHierarchy, OnPingSubSceneInHierarchy);
            evt.menu.AppendAction(k_PingSubSceneInProjectWindow, OnPingSubSceneAsset);
        }

        void OnPingSubSceneInHierarchy(DropdownMenuAction obj)
            => EditorGUIUtility.PingObject(m_Item.NodeId.Id);

        void OnPingSubSceneAsset(DropdownMenuAction obj)
        {
            var subSceneObject = EditorUtility.InstanceIDToObject(m_Item.NodeId.Id);
            if (subSceneObject == null || !subSceneObject || !(subSceneObject is GameObject subSceneGameObject))
                return;

            var subScene = subSceneGameObject.GetComponent<SubScene>();
            if (subScene == null || !subScene || subScene.SceneAsset == null || !subScene.SceneAsset)
                return;

            EditorGUIUtility.PingObject(subScene.SceneAsset);
        }
    }
}
