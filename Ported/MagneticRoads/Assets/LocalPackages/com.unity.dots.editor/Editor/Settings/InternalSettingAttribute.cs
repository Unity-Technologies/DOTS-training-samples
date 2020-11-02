using System;

namespace Unity.Entities.Editor
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    class InternalSettingAttribute : Attribute
    {
    }
}
