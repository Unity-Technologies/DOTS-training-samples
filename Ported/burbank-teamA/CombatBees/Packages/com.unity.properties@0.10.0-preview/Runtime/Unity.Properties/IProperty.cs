namespace Unity.Properties
{
    public interface IProperty
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string GetName();

        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets a value indicating whether the value type is a container type.
        /// </summary>
        bool IsContainer { get; }

        /// <summary>
        /// Returns the attributes for the given property.
        /// </summary>
        IPropertyAttributeCollection Attributes { get; }
    }

    public interface IProperty<TContainer, TValue> : IProperty
    {
        /// <summary>
        /// Gets the property value for the given container.
        /// </summary>
        TValue GetValue(ref TContainer container);

        /// <summary>
        /// Sets the property value for the given container.
        /// </summary>
        void SetValue(ref TContainer container, TValue value);
    }
}
