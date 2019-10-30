using System;
using System.Runtime.InteropServices;

namespace Unity.Properties.Tests
{
    /// <summary>
    /// Simple struct container.
    ///
    /// Layout and field offsets are used here since we are NOT using codegen or reflection.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TestPrimitiveContainer
    {
        [FieldOffset(0)] public bool BoolValue;

        [FieldOffset(1)] public sbyte Int8Value;

        [FieldOffset(2)] public short Int16Value;

        [FieldOffset(4)] public int Int32Value;

        [FieldOffset(8)] public long Int64Value;

        [FieldOffset(16)] public byte UInt8Value;

        [FieldOffset(17)] public ushort UInt16Value;

        [FieldOffset(20)] public uint UInt32Value;

        [FieldOffset(24)] public ulong UInt64Value;

        [FieldOffset(32)] public float Float32Value;

        [FieldOffset(36)] public double Float64Value;

        [FieldOffset(44)] public FlagsEnum FlagsEnum;

        [FieldOffset(48)] public UnorderedIntEnum UnorderedIntEnum;
        
        [FieldOffset(52)] public SmallEnum SmallEnum;
    }

    [Flags]
    public enum FlagsEnum
    {
        None =   0,
        Value1 = 1,
        Value2 = 2,
        Value3 = 4,
        Value4 = 8
    }

    public enum UnorderedIntEnum : int
    {
        None = 0,
        Value1 = 1,
        Value4 = 4,
        Value2 = 2,
        Value3 = 3
    }

    public enum SmallEnum : byte
    {
        None = 0,
        Value1 = 1,
        Value2 = 2,
    }
    
    /// <summary>
    /// Manually written property bag. 
    /// </summary>
    public class TestPrimitiveContainerPropertyBag : PropertyBag<TestPrimitiveContainer>
    {
        readonly UnmanagedProperty<TestPrimitiveContainer, bool> m_BoolValue = new UnmanagedProperty<TestPrimitiveContainer, bool>(
            nameof(TestPrimitiveContainer.BoolValue),
            0);

        readonly UnmanagedProperty<TestPrimitiveContainer, sbyte> m_Int8Value = new UnmanagedProperty<TestPrimitiveContainer, sbyte>(
            nameof(TestPrimitiveContainer.Int8Value),
            1);

        readonly UnmanagedProperty<TestPrimitiveContainer, short> m_Int16Value = new UnmanagedProperty<TestPrimitiveContainer, short>(
            nameof(TestPrimitiveContainer.Int16Value),
            2);

        readonly UnmanagedProperty<TestPrimitiveContainer, int> m_Int32Value = new UnmanagedProperty<TestPrimitiveContainer, int>(
            nameof(TestPrimitiveContainer.Int32Value),
            4);

        readonly UnmanagedProperty<TestPrimitiveContainer, long> m_Int64Value = new UnmanagedProperty<TestPrimitiveContainer, long>(
            nameof(TestPrimitiveContainer.Int64Value),
            8);

        readonly UnmanagedProperty<TestPrimitiveContainer, byte> m_UInt8Value = new UnmanagedProperty<TestPrimitiveContainer, byte>(
            nameof(TestPrimitiveContainer.UInt8Value),
            16);

        readonly UnmanagedProperty<TestPrimitiveContainer, ushort> m_UInt16Value = new UnmanagedProperty<TestPrimitiveContainer, ushort>(
            nameof(TestPrimitiveContainer.UInt16Value),
            17);

        readonly UnmanagedProperty<TestPrimitiveContainer, uint> m_UInt32Value = new UnmanagedProperty<TestPrimitiveContainer, uint>(
            nameof(TestPrimitiveContainer.UInt32Value),
            20);

        readonly UnmanagedProperty<TestPrimitiveContainer, ulong> m_UInt64Value = new UnmanagedProperty<TestPrimitiveContainer, ulong>(
            nameof(TestPrimitiveContainer.UInt64Value),
            24);

        readonly UnmanagedProperty<TestPrimitiveContainer, float> m_Float32Value = new UnmanagedProperty<TestPrimitiveContainer, float>(
            nameof(TestPrimitiveContainer.Float32Value),
            32);

        readonly UnmanagedProperty<TestPrimitiveContainer, double> m_Float64Value = new UnmanagedProperty<TestPrimitiveContainer, double>(
            nameof(TestPrimitiveContainer.Float64Value),
            36);

        readonly UnmanagedProperty<TestPrimitiveContainer, FlagsEnum> m_FlagsEnum = new UnmanagedProperty<TestPrimitiveContainer, FlagsEnum>(
            nameof(TestPrimitiveContainer.FlagsEnum),
            44);

        readonly UnmanagedProperty<TestPrimitiveContainer, UnorderedIntEnum> m_UnorderedIntEnum = new UnmanagedProperty<TestPrimitiveContainer, UnorderedIntEnum>(
            nameof(TestPrimitiveContainer.UnorderedIntEnum),
            48);
        
        readonly UnmanagedProperty<TestPrimitiveContainer, SmallEnum> m_SmallEnum = new UnmanagedProperty<TestPrimitiveContainer, SmallEnum>(
            nameof(TestPrimitiveContainer.SmallEnum),
            52);

        public override void Accept<TVisitor>(ref TestPrimitiveContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
        {
            VisitUnmanagedValueProperty(ref visitor, m_BoolValue, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_Int8Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_Int16Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_Int32Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_Int64Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_UInt8Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_UInt16Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_UInt32Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_UInt64Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_Float32Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_Float64Value, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_FlagsEnum, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_UnorderedIntEnum, ref container, ref changeTracker);
            VisitUnmanagedValueProperty(ref visitor, m_SmallEnum, ref container, ref changeTracker);
        }

        void VisitUnmanagedValueProperty<TVisitor, TValue>(ref TVisitor visitor, UnmanagedProperty<TestPrimitiveContainer, TValue> property, ref TestPrimitiveContainer container,
            ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor
            where TValue : unmanaged
        {
            visitor.VisitProperty<UnmanagedProperty<TestPrimitiveContainer, TValue>, TestPrimitiveContainer, TValue>(property, ref container, ref changeTracker);
        }

        public override bool FindProperty<TCallback>(string name, ref TestPrimitiveContainer container, ref ChangeTracker changeTracker, ref TCallback action)
        {
            return TryFindProperty(name, m_BoolValue, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_Int8Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_Int16Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_Int32Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_Int64Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_UInt8Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_UInt16Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_UInt32Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_UInt64Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_Float32Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_Float64Value, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_FlagsEnum, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_UnorderedIntEnum, ref container, ref changeTracker, ref action) ||
                   TryFindProperty(name, m_SmallEnum, ref container, ref changeTracker, ref action);
        }

        bool TryFindProperty<TCallback, TValue>(string name, UnmanagedProperty<TestPrimitiveContainer, TValue> property, ref TestPrimitiveContainer container, ref ChangeTracker changeTracker,
            ref TCallback callback)
            where TCallback : IPropertyGetter<TestPrimitiveContainer>
            where TValue : unmanaged
        {
            if (string.Equals(name, property.GetName()))
            {
                callback.VisitProperty<UnmanagedProperty<TestPrimitiveContainer, TValue>, TValue>(property, ref container, ref changeTracker);
                return true;
            }

            return false;
        }
    }
}