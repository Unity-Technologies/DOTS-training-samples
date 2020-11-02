using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Editor.Controls;
using Unity.Entities;
using Unity.Entities.Editor;
using Unity.Mathematics;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer : PropertyVisitor
    {
        struct PathScope : IDisposable
        {
            readonly RuntimeComponentsDrawer m_Drawer;
            public PathScope(RuntimeComponentsDrawer context) => m_Drawer = context;
            public void Dispose() => m_Drawer.m_Path.Pop();
        }

        static readonly GUIContent s_PropertyLabelContent = new GUIContent();
        static readonly GUIContent s_PropertyValueContent = new GUIContent();

        /// <summary>
        /// Number of elements to draw before pagination starts.
        /// </summary>
        const int k_BufferPageLength = 5;

        /// <summary>
        /// Pagination state for each buffer element.
        /// </summary>
        /// <remarks>
        /// The key is the hash code of <see cref="DynamicBufferContainer{TElement}"/>.
        /// </remarks>
        readonly Dictionary<int, PaginationField> m_Pagination = new Dictionary<int, PaginationField>();

        /// <summary>
        /// Set of entity targets we are currently inspecting.
        /// </summary>
        readonly List<EntityContainer> m_Targets = new List<EntityContainer>();

        /// <summary>
        /// Set of components that are currently selected in the UI.
        /// </summary>
        readonly HashSet<int> m_SelectedComponentTypes = new HashSet<int>();

        /// <summary>
        /// Property path maintained as we visit. This is used to resolve mixed values across all containers.
        /// </summary>
        readonly PropertyPath m_Path = new PropertyPath();

        /// <summary>
        /// Invoked when the user clicks the deselect component button.
        /// </summary>
        public event Action<int> OnDeselectComponent;

        public RuntimeComponentsDrawer()
        {
            AddAdapter(this);
        }

        /// <summary>
        /// Sets the entity targets that should be inspected.
        /// </summary>
        /// <param name="targets"></param>
        public void SetTargets(IEnumerable<EntityContainer> targets)
        {
            m_Targets.Clear();

            foreach (var target in targets)
            {
                m_Targets.Add(target);
            }
        }

        /// <summary>
        /// Sets the component types which should be drawn.
        /// </summary>
        /// <param name="components"></param>
        public void SetComponentTypes(IEnumerable<int> components)
        {
            m_SelectedComponentTypes.Clear();

            foreach (var component in components)
            {
                m_SelectedComponentTypes.Add(component);
            }
        }

        /// <summary>
        /// Main draw method.
        /// </summary>
        public void OnGUI()
        {
            m_Targets.RemoveAll(data => !data.EntityManager.Exists(data.Entity));

            if (m_Targets.Count == 0)
                return;

            var target = m_Targets.First();

            PropertyContainer.Visit(ref target, this);
        }

        /// <summary>
        /// Invoked by the <see cref="PropertyVisitor"/> base class when encountering ANY property value not consumed by an adapter.
        /// </summary>
        protected override void VisitProperty<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
        {
            if (!BeginProperty(property, value)) return;

            property.Visit(this, ref value);

            EndProperty(property);
        }

        protected override void VisitCollection<TContainer, TCollection, TElement>(Property<TContainer, TCollection> property, ref TContainer container, ref TCollection value)
        {
            if (!BeginProperty(property, value)) return;

            LabelField("Size", value.Count, IsMixedSize<TCollection, TElement>(value));

            var count = GetMinCount<TCollection, TElement>(value);

            EditorGUI.indentLevel++;

            if (count <= k_BufferPageLength)
            {
                for (var i = 0; i < count; i++)
                {
                    property.Visit(this, ref value, i);
                }
            }
            else
            {
                if (!m_Pagination.TryGetValue(container.GetHashCode(), out var scrollData))
                {
                    scrollData = new PaginationField { ItemsPerPage = k_BufferPageLength };
                    m_Pagination.Add(container.GetHashCode(), scrollData);
                }

                scrollData.Count = count;

                var start = scrollData.Page * k_BufferPageLength;
                var end = (scrollData.Page + 1) * k_BufferPageLength;

                for (var index = start; index < end && index < count; index++)
                {
                    property.Visit(this, ref value, index);
                }

                scrollData.OnGUI();
            }

            EditorGUI.indentLevel--;

            EndProperty(property);
        }

        protected override void VisitSet<TContainer, TSet, TElement>(Property<TContainer, TSet> property, ref TContainer container, ref TSet value)
        {
            if (!BeginProperty(property, value)) return;

            if (IsMixedSize<TSet, TElement>(value))
            {
                EditorGUILayout.LabelField(property.Name, "No support for mixed value sets.");
            }
            else
            {
                LabelField("Size", value.Count, false);
                EditorGUI.indentLevel++;
                property.Visit(this, ref value);
                EditorGUI.indentLevel--;
            }

            EndProperty(property);
        }

        protected override void VisitDictionary<TContainer, TDictionary, TKey, TValue>(Property<TContainer, TDictionary> property, ref TContainer container, ref TDictionary value)
        {
            if (!BeginProperty(property, value)) return;


            if (IsMixedSize<TDictionary, KeyValuePair<TKey, TValue>>(value))
            {
                EditorGUILayout.LabelField(property.Name, "No support for mixed value dictionary.");
            }
            else
            {
                LabelField("Size", value.Count, false);
                EditorGUI.indentLevel++;
                property.Visit(this, ref value);
                EditorGUI.indentLevel--;
            }

            EndProperty(property);
        }

        bool BeginProperty<TContainer, TValue>(Property<TContainer, TValue> property, TValue value)
        {
            if (typeof(TContainer) == typeof(EntityContainer))
            {
                var typeIndex = TypeManager.GetTypeIndex(GetComponentType(typeof(TValue)));

                if (!m_SelectedComponentTypes.Contains(typeIndex)) return false;

                var style = new GUIStyle("HelpBox")
                {
                    normal =
                    {
                        background = EditorIcons.RoundedCorners
                    }
                };

                GUI.color = EditorGUIUtility.isProSkin
                    ? new Color32(0x22, 0x22, 0x22, 0xFF)
                    : new Color32(0xE4, 0xE4, 0xE4, 0xFF);

                EditorGUILayout.BeginVertical(style);

                GUI.color = Color.white;

                var content = new GUIContent
                {
                    text = " " + TypeManager.GetType(typeIndex).Name + " " + GetComponentCategory(typeIndex),
                    image = EditorIcons.RuntimeComponent
                };

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(content, EditorStyles.boldLabel);

                    if (GUILayout.Button(EditorIcons.Remove, EditorStyles.label, GUILayout.Width(16)))
                    {
                        OnDeselectComponent?.Invoke(typeIndex);
                        GUIUtility.ExitGUI();
                    }
                }

                // Always use the name when dealing with component types. Even if they are indexable.
                m_Path.PushName(property.Name);
            }
            else
            {
                m_Path.PushProperty(property);

                if (IsMixedType<TValue>(value))
                {
                    EditorGUILayout.LabelField(new GUIContent(GetDisplayName(property)), Bridge.EditorGUIBridge.mixedValueContent);
                    m_Path.Pop();
                    return false;
                }

                EditorGUILayout.LabelField(GetDisplayName(property));
            }

            EditorGUI.indentLevel++;
            return true;
        }

        void EndProperty<TContainer, TValue>(Property<TContainer, TValue> property)
        {
            EditorGUI.indentLevel--;

            m_Path.Pop();

            if (typeof(TContainer) == typeof(EntityContainer))
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2f);
            }
        }

        static Type GetComponentType(Type type)
        {
            if (typeof(IComponentData).IsAssignableFrom(type) || typeof(ISharedComponentData).IsAssignableFrom(type))
                return type;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DynamicBufferContainer<>))
                return type.GenericTypeArguments[0];

            return null;
        }

        static string GetComponentCategory(int t)
        {
            if (TypeManager.IsManagedComponent(t)) return "(Managed)";
            if (TypeManager.IsSharedComponentType(t)) return "(Shared)";
            return TypeManager.IsBuffer(t) ? "(Buffer)" : string.Empty;
        }

        /// <summary>
        /// Draws the given property with a label and value.
        /// </summary>
        /// <remarks>
        /// The property is checked for mixed values.
        /// </remarks>
        void PropertyField<TValue>(IProperty property, TValue value)
        {
            using (MakePathScope(property))
            {
                LabelField(property, value, IsMixedValue(value));
            }
        }

        /// <summary>
        /// Draws the given property and value with custom formatting.
        /// </summary>
        /// <remarks>
        /// If mixed is true the built in mixedValueContent is shown.
        /// </remarks>
        static void LabelField<TValue>(IProperty property, TValue value, bool isMixed)
        {
            LabelField(GetLabelContent(GetDisplayName(property)), isMixed ? Bridge.EditorGUIBridge.mixedValueContent : GetValueContent(value));
        }

        /// <summary>
        /// Draws the given label and value with custom formatting.
        /// </summary>
        /// <remarks>
        /// If mixed is true the built in mixedValueContent is shown.
        /// </remarks>
        static void LabelField<TValue>(string name, TValue value, bool isMixed)
        {
            LabelField(GetLabelContent(name), isMixed ? Bridge.EditorGUIBridge.mixedValueContent : GetValueContent(value));
        }

        /// <summary>
        /// Draws the given label and content with no custom formatting.
        /// </summary>
        static void LabelField(GUIContent label, GUIContent content)
        {
            var richText = EditorStyles.label.richText;
            EditorStyles.label.richText = true;
            EditorGUILayout.LabelField(label, content);
            EditorStyles.label.richText = richText;
        }

        PathScope MakePathScope(string name)
        {
            m_Path.PushName(name);
            return new PathScope(this);
        }

        PathScope MakePathScope(IProperty property)
        {
            m_Path.PushProperty(property);
            return new PathScope(this);
        }

        bool IsMixedValue<TValue>(IProperty property, TValue value)
        {
            using (MakePathScope(property))
            {
                return IsMixedValue(value);
            }
        }

        bool IsMixedValue<TValue>(string path, TValue value)
        {
            using (MakePathScope(path))
            {
                return IsMixedValue(value);
            }
        }

        bool IsMixedType<TValue>(TValue value)
        {
            for (var index = 1; index < m_Targets.Count; index++)
            {
                var target = m_Targets[index];

                if (!PropertyContainer.TryGetValue<EntityContainer, TValue>(ref target, m_Path, out var otherValue))
                {
                    return true;
                }

                if (value == null)
                {
                    if (null != otherValue)
                    {
                        return true;
                    }

                    continue;
                }

                if (null == otherValue)
                {
                    return true;
                }

                if (value.GetType() != otherValue.GetType())
                {
                    return true;
                }
            }

            return false;
        }

        bool IsMixedValue<TValue>(TValue value)
        {
            for (var index = 1; index < m_Targets.Count; index++)
            {
                var target = m_Targets[index];

                if (!PropertyContainer.TryGetValue<EntityContainer, TValue>(ref target, m_Path, out var otherValue))
                {
                    continue;
                }

                if (value == null)
                {
                    if (null != otherValue)
                    {
                        return true;
                    }

                    continue;
                }

                if (!value.Equals(otherValue))
                {
                    return true;
                }
            }

            return false;
        }

        bool IsMixedSize<TCollection, TElement>(TCollection value) where TCollection : ICollection<TElement>
        {
            for (var index = 1; index < m_Targets.Count; index++)
            {
                var target = m_Targets[index];

                if (!PropertyContainer.TryGetValue<EntityContainer, TCollection>(ref target, m_Path, out var otherValue))
                {
                    return true;
                }

                if (value == null)
                {
                    if (null != otherValue)
                    {
                        return true;
                    }

                    continue;
                }

                if (null == otherValue)
                {
                    return true;
                }

                if (value.Count != otherValue.Count)
                {
                    return true;
                }
            }

            return false;
        }

        int GetMinCount<TCollection, TElement>(TCollection value) where TCollection : ICollection<TElement>
        {
            var min = value.Count;

            for (var index = 1; index < m_Targets.Count; index++)
            {
                var target = m_Targets[index];

                if (!PropertyContainer.TryGetValue<EntityContainer, TCollection>(ref target, m_Path, out var otherValue))
                {
                    continue;
                }

                if (otherValue is ICollection<TElement> otherCollection)
                {
                    min = math.min(min, otherCollection.Count);
                }
            }

            return min;
        }

        static string GetDisplayName(IProperty property)
        {
            switch (property)
            {
                case IListElementProperty listElementProperty:
                    return $"Element {listElementProperty.Index}";
                case IDictionaryElementProperty dictionaryElementProperty:
                    return $"Element {dictionaryElementProperty.ObjectKey}";
                default:
                    return property.Name;
            }
        }

        static GUIContent GetLabelContent(string label)
        {
            s_PropertyLabelContent.text = label;
            return s_PropertyLabelContent;
        }

        static GUIContent GetValueContent<TValue>(TValue value)
        {
            string label;

            switch (value as object)
            {
                case null:
                    label = "null";
                    break;
                case IFormattable formattable:
                    label = formattable.ToString("0.##", NumberFormatInfo.CurrentInfo);
                    break;
                default:
                    label = value.ToString();
                    break;
            }

            s_PropertyValueContent.text = $"<b>{label}</b>";
            return s_PropertyValueContent;
        }
    }
}
