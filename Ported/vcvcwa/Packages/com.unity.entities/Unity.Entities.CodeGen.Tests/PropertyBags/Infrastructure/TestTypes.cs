using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Unity.Entities.CodeGen.Tests.PropertyBags.Types
{
#pragma warning disable 0649
    public struct ValueTypeIComponentData : IComponentData
    {
        public int IntField;
    }

    public struct ValueTypeSharedComponentData : ISharedComponentData, IEquatable<ValueTypeSharedComponentData>
    {
        public int IntField;

        public bool Equals(ValueTypeSharedComponentData other)
        {
            return IntField == other.IntField;
        }

        public override int GetHashCode()
        {
            return IntField;
        }
    }
    
#if !UNITY_DISABLE_MANAGED_COMPONENTS
    public class ManagedIComponentData : IComponentData
    {
        public string StringField;
    }
#endif
}

#if !UNITY_DISABLE_MANAGED_COMPONENTS
public class SomeClass
{

}

namespace Some.Namespace
{
    class MyComponent : IComponentData
    {
        public int MyInt;
        public List<string> MyList;
        public SomeClass[] MyArray;
    }
}
#endif
#pragma warning restore 0649