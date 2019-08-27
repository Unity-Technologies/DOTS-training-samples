using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Unity.Entities.Editor
{
    internal class EntityIMGUIVisitor : PropertyVisitor
    {
        private class VisitorStyles
        {
            
            public GUIStyle EntityStyle { get; private set; }
            public GUIContent PageLabel { get; private set; }

            public VisitorStyles()
            {
                EntityStyle = new GUIStyle(EditorStyles.label)
                {
                    normal =
                    {
                        textColor = new Color(0.5f, 0.5f, 0.5f)
                    },
                    onHover =
                    {
                        textColor = new Color(0.0f, 0.7f, 0.7f)
                    }
                };
                PageLabel = new GUIContent(L10n.Tr("Page"), L10n.Tr("Use the slider to navigate pages of buffer elements"));
            }
        }

        private static VisitorStyles Styles;

        private static void InitStyles()
        {
            if (Styles == null)
            {
                Styles = new VisitorStyles();
            }
        }
        
        private class EntityIMGUIAdapter : IMGUIAdapter
            , IVisitAdapter<Entity>
        {

            private readonly EntityDoubleClick m_Callback;

            public EntityIMGUIAdapter(EntityDoubleClick callback)
            {
                m_Callback = callback;
            }

            public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref Entity value, ref ChangeTracker changeTracker) where TProperty : IProperty<TContainer, Entity>
            {
                InitStyles();

                GUI.enabled = true;

                var pos = EditorGUILayout.GetControlRect();

                EditorGUI.LabelField(pos, $"{property.GetName()} Index: {value.Index}, Version: {value.Version}", Styles.EntityStyle);

                if (Event.current.type == EventType.MouseDown && pos.Contains(Event.current.mousePosition))
                {
                    if (Event.current.clickCount == 2)
                    {
                        Event.current.Use();
                        m_Callback?.Invoke(value);
                    }
                }

                GUI.enabled = false;
                return VisitStatus.Handled;
            }
        }

        private class ScrollData
        {
            public float PageHeight;
            public int Page
            {
                get => m_Page;
                set => m_Page = math.max(0, math.min(value, LastPage));
            }
            private int m_Page;

            public int Count
            {
                get => m_Count;
                set
                {
                    m_Count = value;
                    Page = math.min(Page, LastPage);
                }
            }
            private int m_Count = 0;

            public int LastPage => (Count - 1) / kBufferPageLength;
        }
        
        private readonly Dictionary<int, ScrollData> m_ScrollDatas = new Dictionary<int, ScrollData>();

        private const int kBufferPageLength = 5;

        public delegate void EntityDoubleClick(Entity entity);

        public EntityIMGUIVisitor(EntityDoubleClick entityDoubleClick)
        {
            AddAdapter(new IMGUIPrimitivesAdapter());
            AddAdapter(new IMGUIMathematicsAdapter());
            AddAdapter(new EntityIMGUIAdapter(entityDoubleClick));
        }

        protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            if (typeof(TValue).IsEnum)
            {
                var options = Enum.GetNames(typeof(TValue)).ToArray();
                var local = value;
                EditorGUILayout.Popup(
                    typeof(TValue).Name,
                    Array.FindIndex(options, name => name == local.ToString()),
                    options);
            }
            else
            {
                GUILayout.Label(property.GetName());
            }

            return VisitStatus.Handled;
        }

        protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            var enabled = GUI.enabled;
            GUI.enabled = true;
            var foldout = ContainerHeader<TValue>(property.GetName());
            GUI.enabled = enabled;

            EditorGUI.indentLevel++;
            return foldout ? VisitStatus.Handled : VisitStatus.Override;
        }

        private static bool ContainerHeader<TValue>(string displayName)
        {
            EditorGUILayout.LabelField(displayName, new GUIStyle(EditorStyles.boldLabel) { fontStyle = FontStyle.Bold });
            return !typeof(IComponentData).IsAssignableFrom(typeof(TValue)) || !TypeManager.IsZeroSized(TypeManager.GetTypeIndex<TValue>());
        }

        protected override void EndContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            EditorGUI.indentLevel--;
        }

        protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
        {
            InitStyles();
            var count = property.GetCount(ref container);

            // If the count is greater then some arbitrary value
            if (container is IDynamicBufferContainer bufferContainer && count > kBufferPageLength)
            {
                var hash = bufferContainer.GetHashCode();
                if (!m_ScrollDatas.ContainsKey(hash))
                {
                    m_ScrollDatas.Add(hash, new ScrollData());
                }

                var scrollData = m_ScrollDatas[hash];
                scrollData.Count = count;
                
                var oldEnabled = GUI.enabled;
                GUI.enabled = true;
                scrollData.Page = EditorGUILayout.IntSlider(Styles.PageLabel, scrollData.Page, 0, scrollData.LastPage);
                GUI.enabled = oldEnabled;
                
                // We will take control of this branch at this point by by returning `Override`
                // Run the collection visitation manually.
                GUILayout.BeginVertical(GUILayout.MinHeight(scrollData.PageHeight));
                for (var i = scrollData.Page*kBufferPageLength; i < (scrollData.Page+1)*kBufferPageLength && i < count; i++)
                {
                    var callback = new VisitCollectionElementCallback<TContainer>(this);
                    property.GetPropertyAtIndex(ref container, i, ref changeTracker, callback);
                }
                GUILayout.EndVertical();
                scrollData.PageHeight = math.max(scrollData.PageHeight, GUILayoutUtility.GetLastRect().height);
                EndCollection(property, ref container, ref value, ref changeTracker);
                
                return VisitStatus.Override;
            }

            // Otherwise business as usual.
            return VisitStatus.Handled;
        }
        
        internal struct VisitCollectionElementCallback<TContainer> : ICollectionElementPropertyGetter<TContainer>
        {
            private readonly IPropertyVisitor m_Visitor;
        
            public VisitCollectionElementCallback(IPropertyVisitor visitor)
            {
                m_Visitor = visitor;
            }

            public void VisitProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TElementProperty : ICollectionElementProperty<TContainer, TElement>
            {
                m_Visitor.VisitProperty<TElementProperty, TContainer, TElement>(property, ref container, ref changeTracker);
            }

            public void VisitCollectionProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TElementProperty : ICollectionProperty<TContainer, TElement>, ICollectionElementProperty<TContainer, TElement>
            {
                m_Visitor.VisitCollectionProperty<TElementProperty, TContainer, TElement>(property, ref container, ref changeTracker);
            }
        }
    }
}
