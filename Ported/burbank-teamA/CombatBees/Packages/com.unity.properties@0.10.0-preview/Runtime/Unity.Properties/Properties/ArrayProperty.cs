using System;

namespace Unity.Properties
{
    public struct ArrayProperty<TContainer, TElement> : ICollectionProperty<TContainer, TElement[]>
    {
        readonly struct CollectionElementProperty : ICollectionElementProperty<TContainer, TElement>
        {
            readonly ArrayProperty<TContainer, TElement> m_Property;

            public string GetName() => "[" + Index + "]";
            public bool IsReadOnly => false;
            public bool IsContainer { get; }
            public IPropertyAttributeCollection Attributes { get; }
            public int Index { get; }

            public CollectionElementProperty(ArrayProperty<TContainer, TElement> property, int index, IPropertyAttributeCollection attributes = null)
            {
                m_Property = property;
                IsContainer = RuntimeTypeInfoCache<TElement>.IsContainerType();
                Attributes = attributes;
                Index = index;
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

        public delegate TElement[] Getter(ref TContainer container);
        public delegate void Setter(ref TContainer container, TElement[] value);

        readonly string m_Name;
        readonly Getter m_Getter;
        readonly Setter m_Setter;

        public string GetName() => m_Name;
        public bool IsReadOnly => false;
        public bool IsContainer => false;
        public IPropertyAttributeCollection Attributes { get; }

        public ArrayProperty(string name, Getter getter, Setter setter, IPropertyAttributeCollection attributes = null)
        {
            m_Name = name;
            m_Getter = getter;
            m_Setter = setter;
            Attributes = attributes;

            if (RuntimeTypeInfoCache<TElement>.IsArray())
            {
                throw new Exception("ArrayProperty`2 does not support array of array");
            }
        }

        public TElement[] GetValue(ref TContainer container)
        {
            return m_Getter(ref container);
        }

        public void SetValue(ref TContainer container, TElement[] value)
        {
            m_Setter(ref container, value);
        }

        public int GetCount(ref TContainer container)
        {
            return m_Getter(ref container)?.Length ?? 0;
        }

        public void SetCount(ref TContainer container, int count)
        {
            var src = m_Getter(ref container);
            var dst = new TElement[count];

            if (null != src)
            {
                count = Math.Min(src.Length, count);
                for (var i = 0; i < count; i++)
                {
                    dst[i] = src[i];
                }
            }

            m_Setter(ref container, dst);
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
            getter.VisitProperty<CollectionElementProperty, TElement>(new CollectionElementProperty(this, index, Attributes), ref container, ref changeTracker);
        }
    }
}
