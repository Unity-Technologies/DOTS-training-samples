using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Unity.Properties.Reflection
{
    readonly struct ReflectedGenericListProperty<TContainer, TValue, TElement> : ICollectionProperty<TContainer, TValue> 
        where TValue : IList<TElement>
    {
        readonly struct ReflectedListElementProperty : ICollectionElementProperty<TContainer, TElement>
        {
            readonly ReflectedGenericListProperty<TContainer, TValue, TElement> m_Property;
            readonly IPropertyAttributeCollection m_Attributes;
            readonly int m_Index;

            public string GetName() => "[" + Index + "]";
            public bool IsReadOnly => false;
            public bool IsContainer => RuntimeTypeInfoCache<TElement>.IsContainerType();
            public IPropertyAttributeCollection Attributes => m_Attributes;
            public int Index => m_Index;

            public ReflectedListElementProperty(ReflectedGenericListProperty<TContainer, TValue, TElement> property, int index, IPropertyAttributeCollection attributes = null)
            {
                m_Property = property;
                m_Attributes = attributes;
                m_Index = index;
            }

            public TElement GetValue(ref TContainer container) => m_Property.GetValue(ref container)[Index];
            public void SetValue(ref TContainer container, TElement value) => m_Property.GetValue(ref container)[Index] = value;
        }

        readonly IMemberInfo m_Info;

        public string GetName() => m_Info.Name;
        public bool IsReadOnly { get; }
        public bool IsContainer => !(m_Info.PropertyType.IsPrimitive || m_Info.PropertyType.IsEnum || m_Info.PropertyType == typeof(string));
        public IPropertyAttributeCollection Attributes { get; }

        public ReflectedGenericListProperty(IMemberInfo info)
        {
            m_Info = info;
            Attributes = new PropertyAttributeCollection(info.GetCustomAttributes().ToArray());
            IsReadOnly = Attributes.HasAttribute<ReadOnlyAttribute>() || !info.CanWrite();
        }

        public TValue GetValue(ref TContainer container) => (TValue) m_Info.GetValue(container);

        public void SetValue(ref TContainer container, TValue value)
        {
            var boxed = (object) container;
            m_Info.SetValue(boxed, value);
            container = (TContainer) boxed;
        }

        public int GetCount(ref TContainer container) => GetValue(ref container).Count;

        public void SetCount(ref TContainer container, int count)
        {
            var list = GetValue(ref container);

            if (list.Count == count)
            {
                return;
            }

            if (list.Count < count)
            {
                for (var i = list.Count; i < count; i++)
                    list.Add(default);
            }
            else
            {
                for (var i = list.Count - 1; i >= count; i--)
                    list.RemoveAt(i);
            }
        }

        public void Clear(ref TContainer container) => GetValue(ref container).Clear();

        public void GetPropertyAtIndex<TGetter>(ref TContainer container, int index, ref ChangeTracker changeTracker, ref TGetter getter) 
            where TGetter : ICollectionElementPropertyGetter<TContainer>
        {
            getter.VisitProperty<ReflectedListElementProperty, TElement>(new ReflectedListElementProperty(this, index, Attributes), ref container, ref changeTracker);
        }
    }
}