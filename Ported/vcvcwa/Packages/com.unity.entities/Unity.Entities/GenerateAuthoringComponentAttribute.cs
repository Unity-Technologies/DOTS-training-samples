using System;
using UnityEngine;

namespace Unity.Entities
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class GenerateAuthoringComponentAttribute : Attribute
    {
    }

#if !UNITY_DOTSPLAYER
    [AttributeUsage(AttributeTargets.Field)]
    public class RestrictAuthoringInputToAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public RestrictAuthoringInputToAttribute(Type type)
        {
            Type = type;
        }
    }
#endif
}