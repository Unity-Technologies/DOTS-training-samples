using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using Unity.Properties.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// This declares a custom inspector for the <see cref="EntitySelectionProxy"/> that can be used to override the
    /// default inspector.
    /// </summary>
    class EntityEditor : UnityEditor.Editor, IBinding
    {
        static readonly List<EntityEditor> s_Editors = new List<EntityEditor>();

        internal readonly EntityInspectorContext m_Context = new EntityInspectorContext();
        readonly List<string> m_Filters = new List<string>();
        readonly EntityInspectorComponentOrder m_CurrentComponentOrder = new EntityInspectorComponentOrder();
        EntityInspectorStructureVisitor m_UpdateVisitor = new EntityInspectorStructureVisitor();

        BindableElement m_Root;
        VisualElement m_ComponentsRoot;
        TagComponentContainer m_TagsRoot;
        EntityInspectorVisitor m_InspectorVisitor;
        ToolbarSearchField m_SearchField;
        ToolbarMenu m_Settings;

        EntitySelectionProxy Target => (EntitySelectionProxy)target;

        [RootEditor(supportsAddComponent: false), UsedImplicitly]
        public static Type GetEditor(UnityObject[] targets)
        {
            using (var pooled = targets.OfType<EntitySelectionProxy>().ToPooledList())
            {
                var list = pooled.List;
                if (list.Count == 0)
                    return null;

                var proxy = list[0];
                if (!proxy.Exists)
                    return null;
            }

            return InspectorUtility.Settings.Backend == InspectorSettings.InspectorBackend.Debug
                ? null
                : typeof(EntityEditor);
        }

        void OnEnable()
        {
            s_Editors.Add(this);
            m_Root = new BindableElement() { name = "Entity Inspector", binding = this };
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        void OnDisable()
        {
            s_Editors.Remove(this);
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        }

        void OnBeforeAssemblyReload()
        {
            // After a domain reload, it's impossible to retrieve the Entity this editor is
            // referring to. So before the domain is unloaded, we ensure that:
            // 1. The active selection is not an EntitySelectionProxy, so we don't try to
            // re-select it once the domain is reloaded.
            if (Selection.activeObject is EntitySelectionProxy)
                Selection.activeObject = null;

            // 2. This editor no longer exists, so a locked inspector is not revived with
            // invalid data once the domain is reloaded.
            DestroyImmediate(this);
        }

        public override bool UseDefaultMargins() => false;

        protected override void OnHeaderGUI()
        {
            // Intentionally overriden to avoid the default header to be drawn.
        }

        public override VisualElement CreateInspectorGUI()
        {
            Resources.Templates.Inspector.InspectorStyle.AddStyles(m_Root);
            Resources.Templates.DotsEditorCommon.AddStyles(m_Root);
            Initialize(target as EntitySelectionProxy);
            return m_Root;
        }

        void IBinding.PreUpdate()
        {
            if (!m_Context.TargetExists())
            {
                m_Root.Clear();
                return;
            }

            m_UpdateVisitor.Reset();
            var container = m_Context.EntityContainer;
            PropertyContainer.Visit(ref container, m_UpdateVisitor);
            UpdateComponentOrder(m_UpdateVisitor.ComponentOrder);
        }

        void IBinding.Update()
        {
            StylingUtility.AlignInspectorLabelWidth(m_ComponentsRoot);
        }

        void IBinding.Release()
        {
            // Nothing to do
        }

        void Initialize(EntitySelectionProxy proxy)
        {
            m_Context.SetContext(proxy);
            m_Root.Clear();

            var header = new PropertyElement();
            header.AddContext(m_Context);
            header.SetTarget(new EntityHeader(m_Context));
            m_Root.Add(header);
            m_SearchField = header.Q<ToolbarSearchField>();
            m_SearchField.RegisterValueChangedCallback(evt =>
            {
                m_Filters.Clear();
                var value = evt.newValue.Trim();
                var matches = value.Split(' ');
                foreach (var match in matches)
                {
                    m_Filters.Add(match);
                }

                SearchChanged();
            });

            m_Settings = m_Root.Q<ToolbarMenu>();
            // TODO: Remove once we have menu items.
            m_Settings.Hide();

            m_ComponentsRoot = new VisualElement();

            m_Root.Add(m_ComponentsRoot);
            Resources.Templates.Inspector.ComponentsRoot.AddStyles(m_ComponentsRoot);
            m_ComponentsRoot.AddToClassList("entity-inspector__components-root");
            m_ComponentsRoot.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            m_TagsRoot = new TagComponentContainer(m_Context);
            m_ComponentsRoot.Add(m_TagsRoot);

            m_InspectorVisitor = new EntityInspectorVisitor(m_ComponentsRoot, m_TagsRoot, m_Context);
            PropertyContainer.Visit(m_Context.EntityContainer, m_InspectorVisitor);

            m_Root.ForceUpdateBindings();
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            StylingUtility.AlignInspectorLabelWidth(m_ComponentsRoot);
        }

        void SearchChanged()
        {
            using (var pooled = PooledList<ComponentElementBase>.Make())
            {
                var list = pooled.List;
                m_ComponentsRoot.Query<ComponentElementBase>().ToList(list);
                if (m_Filters.Count == 0)
                    list.ForEach(ce => ce.Show());
                else
                {
                    list.ForEach(ce =>
                    {
                        var showShow = false;
                        foreach (var token in m_Filters)
                        {
                            if (ce.Path.IndexOf(token, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                showShow = true;
                                break;
                            }
                        }
                        if (showShow)
                            ce.Show();
                        else
                            ce.Hide();
                    });
                }
            }
        }

        void UpdateComponentOrder(EntityInspectorComponentOrder current)
        {
            m_CurrentComponentOrder.Reset();
            using (var pooledElements = PooledList<ComponentElementBase>.Make())
            {
                ComputeCurrentComponentOrder(m_CurrentComponentOrder, pooledElements);

                if (current == m_CurrentComponentOrder)
                    return;

                // Component removed since the last update
                using (var pooled = ComputeRemovedComponents(current.Components, m_CurrentComponentOrder.Components))
                {
                    var list = pooled.List;
                    foreach (var path in list)
                    {
                        var element = pooledElements.List.Find(ce => ce.Path == path);
                        element?.RemoveFromHierarchy();
                    }
                }

                // Tags removed since the last update
                using (var pooled = ComputeRemovedComponents(current.Tags, m_CurrentComponentOrder.Tags))
                {
                    var list = pooled.List;
                    foreach (var path in list)
                    {
                        var element = pooledElements.List.Find(ce => ce.Path == path);
                        element?.RemoveFromHierarchy();
                    }
                }

                // Component added since the last update
                using (var pooled = ComputeAddedComponents(current.Components, m_CurrentComponentOrder.Components))
                {
                    var list = pooled.List;
                    var container = Target.Container;
                    foreach (var path in list)
                    {
                        PropertyContainer.Visit(ref container, m_InspectorVisitor, new PropertyPath(path));
                    }
                }

                // Tags removed since the last update
                using (var pooled = ComputeAddedComponents(current.Tags, m_CurrentComponentOrder.Tags))
                {
                    var list = pooled.List;
                    var container = Target.Container;
                    foreach (var path in list)
                    {
                        PropertyContainer.Visit(ref container, m_InspectorVisitor, new PropertyPath(path));
                    }
                }
            }
        }

        void ComputeCurrentComponentOrder(EntityInspectorComponentOrder info, List<ComponentElementBase> elements)
        {
            elements.Clear();
            m_ComponentsRoot.Query<ComponentElementBase>().ToList(elements);
            foreach (var ce in elements)
            {
                if (ce.Type == ComponentPropertyType.Tag)
                    info.Tags.Add(ce.Path);
                else
                    info.Components.Add(ce.Path);
            }
        }

        static PooledList<string> ComputeAddedComponents(IEnumerable<string> lhs, IEnumerable<string> rhs)
        {
            return Except(lhs, rhs);
        }

        static PooledList<string> ComputeRemovedComponents(IEnumerable<string> lhs, IEnumerable<string> rhs)
        {
            return Except(rhs, lhs);
        }

        static PooledList<string> Except(IEnumerable<string> lhs, IEnumerable<string> rhs)
        {
            return lhs.Except(rhs).ToPooledList();
        }
    }
}
