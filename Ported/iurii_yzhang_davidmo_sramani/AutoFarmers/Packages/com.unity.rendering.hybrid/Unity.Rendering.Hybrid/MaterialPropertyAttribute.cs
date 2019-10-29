using System;
using UnityEngine;

namespace Unity.Rendering
{
    public enum MaterialPropertyFormat
    {
        Float,
        Float4,
        Float4x4,
    }

    // Use this to mark an IComponentData as an input to a material property on a particular shader.
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
    public class MaterialPropertyAttribute : Attribute
    {
        public MaterialPropertyAttribute(string materialPropertyName, MaterialPropertyFormat format)
        {
            Name = materialPropertyName;
            Format = format;
        }

        public string Name { get; }
        public MaterialPropertyFormat Format { get; }
    }
}
