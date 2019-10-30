namespace Unity.Properties
{
    public struct Property<TContainer, TValue> : IProperty<TContainer, TValue>
    {
        public delegate TValue Getter(ref TContainer container);
        public delegate void Setter(ref TContainer container, TValue value);

        readonly string m_Name;
        readonly Getter m_Getter;
        readonly Setter m_Setter;

        public string GetName() => m_Name;
        public bool IsReadOnly => null == m_Setter;
        public bool IsContainer => RuntimeTypeInfoCache<TValue>.IsContainerType();
        public IPropertyAttributeCollection Attributes { get; }

        public Property(string name, Getter getter, Setter setter = null, IPropertyAttributeCollection attributes = null)
        {
            m_Name = name;
            m_Getter = getter;
            m_Setter = setter;
            Attributes = attributes;
        }

        public TValue GetValue(ref TContainer container)
        {
            return m_Getter(ref container);
        }

        public void SetValue(ref TContainer container, TValue value)
        {
            m_Setter(ref container, value);
        }
    }
}
