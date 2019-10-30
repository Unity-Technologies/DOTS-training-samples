namespace Unity.Properties
{
    public struct ValueProperty<TContainer, TValue> : IProperty<TContainer, TValue>
    {
        public delegate TValue Getter(ref TContainer container);
        public delegate void Setter(ref TContainer container, TValue value);

        readonly string m_Name;
        readonly Getter m_Getter;
        readonly Setter m_Setter;
        readonly IPropertyAttributeCollection m_Attributes;

        public string GetName() => m_Name;
        public bool IsReadOnly => false;
        public bool IsContainer => RuntimeTypeInfoCache<TValue>.IsContainerType();
        public IPropertyAttributeCollection Attributes => m_Attributes;

        public ValueProperty(string name, Getter getter, Setter setter, IPropertyAttributeCollection attributes = null)
        {
            m_Name = name;
            m_Getter = getter;
            m_Setter = setter;
            m_Attributes = attributes;
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
