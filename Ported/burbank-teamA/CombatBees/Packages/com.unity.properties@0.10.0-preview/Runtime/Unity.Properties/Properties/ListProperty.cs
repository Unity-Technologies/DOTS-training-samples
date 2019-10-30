using System;
using System.Collections.Generic;

namespace Unity.Properties
{
    public struct ListProperty<TContainer, TElement> : ICollectionProperty<TContainer, IList<TElement>>
    {
        struct CollectionElementProperty : ICollectionElementProperty<TContainer, TElement>
        {
            readonly ListProperty<TContainer, TElement> m_Property;
            readonly IPropertyAttributeCollection m_Attributes;
            readonly int m_Index;

            public string GetName() => "[" + Index + "]";
            public bool IsReadOnly => false;
            public bool IsContainer => RuntimeTypeInfoCache<TElement>.IsContainerType();
            public IPropertyAttributeCollection Attributes => m_Attributes;
            public int Index => m_Index;

            public CollectionElementProperty(ListProperty<TContainer, TElement> property, int index, IPropertyAttributeCollection attributes = null)
            {
                m_Property = property;
                m_Attributes = attributes;
                m_Index = index;
            }

            public TElement GetValue(ref TContainer container)
            {
                return m_Property.m_Getter(ref container)[Index];
            }

            public void SetValue(ref TContainer container, TElement value)
            {
                m_Property.m_Getter(ref container)[Index] = value;
            }
        }

        public delegate IList<TElement> Getter(ref TContainer container);
        public delegate void Setter(ref TContainer container, IList<TElement> value);

        readonly string m_Name;
        readonly Getter m_Getter;
        readonly Setter m_Setter;
        readonly IPropertyAttributeCollection m_Attributes;

        public string GetName() => m_Name;
        public bool IsReadOnly => false;
        public bool IsContainer => false;
        public IPropertyAttributeCollection Attributes => m_Attributes;

        public ListProperty(string name, Getter getter, Setter setter, IPropertyAttributeCollection attributes = null)
        {
            m_Name = name;
            m_Getter = getter;
            m_Setter = setter;
            m_Attributes = attributes;

            if (RuntimeTypeInfoCache<TElement>.IsArray())
            {
                throw new Exception("ArrayProperty`2 does not support array of array");
            }
        }

        public IList<TElement> GetValue(ref TContainer container)
        {
            return m_Getter(ref container);
        }

        public void SetValue(ref TContainer container, IList<TElement> value)
        {
            m_Setter(ref container, value);
        }

        public int GetCount(ref TContainer container)
        {
            return m_Getter(ref container)?.Count ?? 0;
        }

        public void SetCount(ref TContainer container, int count)
        {
            var list = m_Getter(ref container);

            if (null == list)
            {
                return;
            }

            if (list.Count == count)
            {
                return;
            }

            if (list.Count < count)
            {
                for (var i = list.Count; i < count; i++)
                {
                    list.Add(default);
                }
            }
            else
            {
                for (var i = list.Count - 1; i >= count; i--)
                {
                    list.RemoveAt(i);
                }
            }
        }

        public void Clear(ref TContainer container) 
        {
            if (null == m_Getter(ref container))
            {
                return;
            }

            m_Setter(ref container, new TElement[0]);
        }

        public void GetPropertyAtIndex<TGetter>(ref TContainer container, int index, ref ChangeTracker changeTracker, ref TGetter getter)
            where TGetter : ICollectionElementPropertyGetter<TContainer>
        {
            getter.VisitProperty<CollectionElementProperty, TElement>(new CollectionElementProperty(this, index, m_Attributes), ref container, ref changeTracker);
        }
    }
}
