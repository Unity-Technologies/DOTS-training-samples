using System.Collections.Generic;

namespace Unity.Properties.Tests
{
#pragma warning disable 649
    struct StructContainerWithPrimitives
    {
        public bool BoolValue;
        public sbyte Int8Value;
        public short Int16Value;
        public int Int32Value;
        public long Int64Value;
        public byte UInt8Value;
        public ushort UInt16Value;
        public uint UInt32Value;
        public ulong UInt64Value;
        public float Float32Value;
        public double Float64Value;
        public FlagsEnum FlagsEnum;
        public UnorderedIntEnum UnorderedIntEnum;
        public SmallEnum SmallEnum;
    }

    class ClassContainerWithPrimitives
    {
        public bool BoolValue;
        public sbyte Int8Value;
        public short Int16Value;
        public int Int32Value;
        public long Int64Value;
        public byte UInt8Value;
        public ushort UInt16Value;
        public uint UInt32Value;
        public ulong UInt64Value;
        public float Float32Value;
        public double Float64Value;
        public FlagsEnum FlagsEnum;
        public UnorderedIntEnum UnorderedIntEnum;
        public SmallEnum SmallEnum;
    }

    class ClassContainerWithNestedClass
    {
        public ClassContainerWithPrimitives Container;
    }

    struct StructContainerWithNestedStruct
    {
        public StructContainerWithPrimitives Container;
    }

    struct StructContainerWithNestedDynamicContainer
    {
        public DynamicContainer Container;
    }

    class ClassContainerWithLists
    {
        public List<int> IntList;
        public List<ClassContainerWithPrimitives> ContainerWithPrimitivesList;
    }

    class ClassContainerWithArrays
    {
        public int[] IntArray;
    }

    class ClassContainerWithAbstractField
    {
        public BaseClass Container;
    }

    abstract class BaseClass
    {
        public int BaseIntValue;
    }

    class DerivedClassA : BaseClass
    {
        public int A;
    }

    class DerivedClassB : BaseClass
    {
        public int B;
    }

    interface IContainer
    {
        
    }

    struct StructContainerWithInterface : IContainer
    {
        public int X;
        public int Y;
        public int Z;
    }

    readonly struct DynamicContainer
    {
        public const string TypeIdentifierKey = "$type";

        readonly string m_Type;

        public DynamicContainer(string type) => m_Type = type;

        static DynamicContainer() => PropertyBagResolver.Register(new PropertyBag());

        class PropertyBag : PropertyBag<DynamicContainer>
        {
            static readonly Property<DynamicContainer, string> s_TypeProperty = new Property<DynamicContainer, string>(TypeIdentifierKey, (ref DynamicContainer c) => c.m_Type);

            public override void Accept<TVisitor>(ref DynamicContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
                => visitor.VisitProperty<Property<DynamicContainer, string>, DynamicContainer, string>(s_TypeProperty, ref container, ref changeTracker);

            public override bool FindProperty<TAction>(string name, ref DynamicContainer container, ref ChangeTracker changeTracker, ref TAction action)
            {
                if (!string.Equals(name, TypeIdentifierKey)) return false;
                action.VisitProperty<Property<DynamicContainer, string>, string>(s_TypeProperty, ref container, ref changeTracker);
                return true;
            }
        }
    }
#pragma warning restore 649
}