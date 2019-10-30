using JetBrains.Annotations;

namespace Unity.Properties.Editor
{
    internal struct PropertyWrapper<T>
    {
        [Property, UsedImplicitly]
        private T m_Value;

        public PropertyWrapper(T value)
        {
            m_Value = value;
        }
    }
}