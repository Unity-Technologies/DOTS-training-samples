using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    // TODO: Tons of optimisations can be done here.
    class BindingVisitor : PropertyVisitor
    {
        internal enum BindingRegistration
        {
            None = 0,
            Register = 1,
            Unregister = 2
        }
        
        static readonly List<string> s_NamesBuffer = new List<string>();

        readonly StringBuilder m_PropertyPathBuilder = new StringBuilder(64, 1024);
        readonly Dictionary<string, List<VisualElement>> m_FullPathToElementsMap;
        readonly List<BindableElement> m_BindableElements = new List<BindableElement>();
        readonly BindingRegistration m_Operation = BindingRegistration.None;

        public BindingVisitor(VisualElement root, BindingRegistration bindingRegistration)
            :this(root)
        {
            m_Operation = bindingRegistration;
        }
        
        public BindingVisitor(VisualElement root)
        {
            // We'll find all BindableElements under the root and map it to its full path.
            m_FullPathToElementsMap = new Dictionary<string, List<VisualElement>>();
            root.Query<BindableElement>().Where(s => !string.IsNullOrEmpty(s.bindingPath)).ToList(m_BindableElements);
            
            foreach (var bindable in m_BindableElements)
            {
                if (!m_FullPathToElementsMap.TryGetValue(bindable.bindingPath, out var list))
                {
                    m_FullPathToElementsMap[bindable.bindingPath] = list = new List<VisualElement>();
                }

                list.Add(bindable);

                try
                {
                    s_NamesBuffer.Add(bindable.bindingPath);
                    var parent = bindable.parent;
                    if (null != parent)
                    {
                        while (null != parent && parent != root)
                        {
                            if (parent is IBindable bParent)
                            {
                                if (!string.IsNullOrEmpty(bParent.bindingPath))
                                {
                                    s_NamesBuffer.Add(bParent.bindingPath);
                                }
                            }

                            parent = parent.parent;
                        }

                        if (s_NamesBuffer.Count > 1)
                        {
                            var fullPath = string.Join(".", ((IEnumerable<string>) s_NamesBuffer).Reverse()).Replace(".[", "[");
                            if (!m_FullPathToElementsMap.TryGetValue(fullPath, out var fullPathList))
                            {
                                fullPathList = new List<VisualElement>();
                                m_FullPathToElementsMap[fullPath] = fullPathList;
                            }

                            fullPathList.Add(bindable);
                        }
                    }
                }
                finally
                {
                    s_NamesBuffer.Clear();
                }
            }
        }
        
        internal void GetElementToFullPathMappings(Dictionary<VisualElement, string> map)
        {
            map.Clear();
            foreach (var kvp in m_FullPathToElementsMap)
            {
                foreach (var element in kvp.Value)
                {
                    map[element] = kvp.Key;
                }
            }
        }

        private void Add(string str)
        {
            if (m_PropertyPathBuilder.Length > 0)
            {
                if (!(str.StartsWith("[") && str.EndsWith("]")))
                {
                    m_PropertyPathBuilder.Append('.');
                }
            }

            m_PropertyPathBuilder.Append(str);
        }

        private void Remove(string str)
        {
            var length = m_PropertyPathBuilder.Length;
            if (length == str.Length)
            {
                m_PropertyPathBuilder.Clear();
            }
            else if (str.StartsWith("[") && str.EndsWith("]"))
            {
                m_PropertyPathBuilder.Remove(length - str.Length, str.Length);
            }
            else
            {
                m_PropertyPathBuilder.Remove(length - str.Length - 1, str.Length + 1);
            }
        }

        protected override VisitStatus Visit<TProperty, TContainer, TValue>(
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
            if (typeof(TValue).IsClass && null == value)
            {
                return VisitStatus.Override;
            }
            
            var propName = property.GetName();
            Add(propName);
            try
            {
                if (m_FullPathToElementsMap.TryGetValue(m_PropertyPathBuilder.ToString(), out var fullPathElements))
                {
                    foreach (var element in fullPathElements)
                    {
                        switch (m_Operation)
                        {
                            case BindingRegistration.None:
                                Connectors.SetData(element, value);
                                break;
                            case BindingRegistration.Register:
                                Connectors.RegisterCallback(element, value);
                                break;
                            case BindingRegistration.Unregister:
                                Connectors.UnregisterCallback(element, value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            finally
            {
                Remove(propName);
            }
            return VisitStatus.Handled;
        }

        protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
            if (!typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue)) && typeof(TValue).IsClass && null == value)
            {
                return VisitStatus.Override;
            }
            
            var propName = property.GetName();
            Add(propName);
            try
            {
                if (m_FullPathToElementsMap.TryGetValue(m_PropertyPathBuilder.ToString(), out var fullPathElements))
                {
                    foreach (var element in fullPathElements)
                    {
                        switch (m_Operation)
                        {
                            case BindingRegistration.None:
                                Connectors.SetData(element, value);
                                break;
                            case BindingRegistration.Register:
                                Connectors.RegisterCallback(element, value);
                                break;
                            case BindingRegistration.Unregister:
                                Connectors.UnregisterCallback(element, value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (null != value)
                {
                    PropertyContainer.Visit(ref value, this);
                }
            }
            finally
            {
                Remove(propName);
            }

            return VisitStatus.Override;
        }

        protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
            if (typeof(TValue).IsClass && null == value)
            {
                return VisitStatus.Override;
            }
            
            var propName = property.GetName();
            Add(propName);
            var path = m_PropertyPathBuilder.ToString();
            if (m_FullPathToElementsMap.TryGetValue(path, out var fullPathElements))
            { 
                foreach (var element in fullPathElements)
                {
                    // Special element to track the array size
                    var field = element.Q<IntegerField>("CollectionSize");
                    if (null == field)
                    {
                        continue;
                    }

                    switch (m_Operation)
                    {
                        case BindingRegistration.None:
                            Connectors.SetCollectionSize(field, property.GetCount(ref container));
                            break;
                        case BindingRegistration.Register:
                            // Nothing to register for this.
                            break;
                        case BindingRegistration.Unregister:
                            // Nothing to unregister for this.
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            for (int i = 0, count = property.GetCount(ref container); i < count; i++)
            {
                var callback = new VisitCollectionElementCallback<TContainer>(this);
                var elementChangeTracker = new ChangeTracker(changeTracker.VersionStorage);
                property.GetPropertyAtIndex(ref container, i, ref elementChangeTracker, ref callback);
            }
            Remove(propName);
            return VisitStatus.Override;
        }
    }
}
