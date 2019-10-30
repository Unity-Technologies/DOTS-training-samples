using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Properties
{
    public interface IUnmanagedProperty : IProperty
    {
        int Offset { get; }
    }

    public readonly struct UnmanagedProperty<TContainer, TValue> : IUnmanagedProperty, IProperty<TContainer, TValue>
        where TContainer : struct
        where TValue : unmanaged
    {
        readonly string m_Name;

        public string GetName() => m_Name;
        public int Offset { get; }
        public bool IsReadOnly { get; }
        public bool IsContainer { get; }
        public IPropertyAttributeCollection Attributes { get; }

        public UnmanagedProperty(string name, int offset, bool readOnly = false, IPropertyAttributeCollection attributes = null)
        {
            m_Name = name;
            Offset = offset;
            IsReadOnly = readOnly;
            IsContainer = RuntimeTypeInfoCache<TValue>.IsContainerType();
            Attributes = attributes;
        }

        public unsafe TValue GetValue(ref TContainer container)
            => *(TValue*) ((byte*) UnsafeUtility.AddressOf(ref container) + Offset);

        public unsafe void SetValue(ref TContainer container, TValue value)
            => *(TValue*) ((byte*) UnsafeUtility.AddressOf(ref container) + Offset) = value;
    }
}