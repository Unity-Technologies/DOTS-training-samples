using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    /// <summary>
    /// Allows to know at which level we are inspecting.
    /// </summary>
    public enum InspectorVisitLevel
    {
        /// <summary>
        /// The target of the custom inspector is the root object.
        /// </summary>
        Target,
        /// <summary>
        /// The target of the custom inspector is a (sub-)field of the root object
        /// </summary>
        Field
    }

    /// <summary>
    /// Allows to declare a type as custom inspector for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInspector<T>
    {
        /// <summary>
        /// Called whenever the UI needs to be rebuilt.
        /// </summary>
        /// <param name="context">Context object for the inspector.</param>
        /// <returns>The root visual element to use for the inspection.</returns>
        VisualElement Build(InspectorContext<T> context);
        
        /// <summary>
        /// Called whenever the underlying data changed, so the custom inspector can update it's data.
        /// </summary>
        /// <param name="context">Context object for the inspector.</param>
        void Update(InspectorContext<T> context);
    }

    /// <summary>
    /// Context of the inspector that give access to the data.
    /// </summary>
    /// <typeparam name="T">The type of the value being inspected.</typeparam>
    public readonly struct InspectorContext<T>
    {
        readonly PropertyElement m_PropertyElement;
        readonly PropertyPath m_PropertyPath;
        readonly IProperty m_Property;
        
        public readonly string Name;
        public readonly string PrettyName;
        public readonly string Tooltip;
        public readonly bool IsDelayed;
        public readonly InspectorVisitLevel VisitLevel;

        static readonly IPropertyAttributeCollection Empty = new PropertyAttributeCollection();
 
        public IPropertyAttributeCollection Attributes { get; }
        
        // TODO: Cache the result of ObjectNames.NicifyVariableName

        internal InspectorContext(PropertyElement propertyElement, IProperty property, PropertyPath propertyPath, string name, InspectorVisitLevel visitLevel)
        {
            m_PropertyElement = propertyElement;
            m_PropertyPath = propertyPath;
            Name = name;
            VisitLevel = visitLevel;
            m_Property = property;
            Attributes = m_Property.Attributes ?? Empty;
            Tooltip = Attributes.GetAttribute<TooltipAttribute>()?.tooltip;
            PrettyName = Attributes.GetAttribute<InspectorNameAttribute>()?.displayName ??
                         (!string.IsNullOrEmpty(Name) ? ObjectNames.NicifyVariableName(Name) : Name);
            IsDelayed = Attributes.HasAttribute<DelayedAttribute>();
        }
        
        /// <summary>
        /// Accessor for the data.
        /// </summary>
        public T Data
        {
            get => GetData();
            set => SetData(value);
        }

        /// <summary>
        /// Allows to revert to the default drawing handler for a specific field.  
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="name">The name of the field that needs to be drawn.</param>
        public void DoDefaultGui(VisualElement parent, string name)
        {
            var path = new PropertyPath(m_PropertyPath.ToString());
            path.Append(new PropertyPath(name));
            if (!m_PropertyElement.TryVisitAtPath(parent, path))
            {
                Debug.LogWarning($"Could not visit property at path: `{path}`.");
            }
        }

        public void NotifyChanged()
        {
            m_PropertyElement.NotifyChanged();
        }

        T GetData()
        {
            return m_PropertyPath.PartsCount == 0 ? m_PropertyElement.GetTarget<T>() : m_PropertyElement.GetValueAtPath<T>(m_PropertyPath);
        }

        void SetData(T value)
        {
            if (m_PropertyPath.PartsCount == 0)
            {
                m_PropertyElement.SetTarget(value);
                m_PropertyElement.NotifyChanged();
            }
            else
            {
                m_PropertyElement.SetValueAtPath(m_PropertyPath, value);
            }
        }
    }
}