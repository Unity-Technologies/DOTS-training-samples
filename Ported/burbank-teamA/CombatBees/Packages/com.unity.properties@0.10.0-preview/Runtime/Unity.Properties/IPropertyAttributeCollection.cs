using System;
using System.Collections.Generic;

namespace Unity.Properties
{
    /// <summary>
    /// Interface for accessing attributes for properties.
    /// </summary>
    public interface IPropertyAttributeCollection
    {
        /// <summary>
        /// Returns true if the property has any attributes of the given type.
        /// </summary>
        bool HasAttribute<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Returns the first attribute of the given type.
        /// </summary>
        TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute;

        /// <summary>
        /// Returns all attribute of the given type.
        /// </summary>
        IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute;
    }

    /// <inheritdoc cref="IPropertyAttributeCollection" />
    /// <summary>
    /// Default container to pass property attributes.
    /// </summary>
    public struct PropertyAttributeCollection : IPropertyAttributeCollection
    {
        readonly Attribute[] m_Attributes;

        public PropertyAttributeCollection(Attribute[] attributes)
        {
            m_Attributes = attributes;
        }

        /// <inheritdoc />
        public bool HasAttribute<TAttribute>() where TAttribute : Attribute
        {
            for (var i = 0; i < m_Attributes?.Length; i++)
            {
                if (m_Attributes[i] is TAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            for (var i = 0; i < m_Attributes?.Length; i++)
            {
                if (m_Attributes[i] is TAttribute typed)
                {
                    return typed;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
        {
            for (var i = 0; i < m_Attributes?.Length; i++)
            {
                if (m_Attributes[i] is TAttribute typed)
                {
                    yield return typed;
                }
            }
        }
    }
}
