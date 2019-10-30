using System.Linq;
using Unity.Collections;

namespace Unity.Properties.Reflection
{
    readonly struct ReflectedMemberProperty<TContainer, TValue> : IProperty<TContainer, TValue>
    {
        readonly IMemberInfo m_Info;

        public string GetName() => m_Info.Name;
        public bool IsReadOnly { get; }
        public bool IsContainer => RuntimeTypeInfoCache<TValue>.IsContainerType();
        public IPropertyAttributeCollection Attributes { get; }

        public ReflectedMemberProperty(IMemberInfo info)
        {
            m_Info = info;
            Attributes = new PropertyAttributeCollection(info.GetCustomAttributes().ToArray());
            IsReadOnly = Attributes.HasAttribute<ReadOnlyAttribute>() || !info.CanWrite();
        }

        public TValue GetValue(ref TContainer container)
        {
            return (TValue) m_Info.GetValue(container);
        }

        public void SetValue(ref TContainer container, TValue value)
        {
            var boxed = (object) container;
            m_Info.SetValue(boxed, value);
            container = (TContainer) boxed;
        }
    }
}