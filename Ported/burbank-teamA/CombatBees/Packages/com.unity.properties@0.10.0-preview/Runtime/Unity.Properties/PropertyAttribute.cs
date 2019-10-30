using System;

namespace Unity.Properties
{
    /// <summary>
    /// Use this attribute to have a property generated for the member.
    /// </summary>
    /// <remarks>
    /// By default public fields will have properties generated.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PropertyAttribute: Attribute
    {
        
    }
}