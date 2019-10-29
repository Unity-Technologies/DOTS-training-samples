#if !NET_DOTS
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;

namespace Unity.Entities.Serialization
{

    public unsafe class BinaryPrimitiveWriterAdapter : IPropertyVisitorAdapter
        , IVisitAdapterPrimitives
        , IVisitAdapter<string>
        , IVisitAdapter
    {
        public BinaryPrimitiveWriterAdapter(UnsafeAppendBuffer* buffer)
        {
            m_Buffer = buffer;
        }

        internal UnsafeAppendBuffer* m_Buffer;

        public ref UnsafeAppendBuffer Buffer
        {
            get { return ref *m_Buffer; }
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref sbyte value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, sbyte>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref short value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, short>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref int value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, int>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref long value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, long>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref byte value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, byte>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref ushort value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, ushort>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref uint value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, uint>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref ulong value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, ulong>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref float value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, float>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref double value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, double>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref bool value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, bool>
        {
            m_Buffer->Add((byte)(value ? 1 : 0));
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref char value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, char>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        unsafe public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref string value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, string>
        {
            m_Buffer->Add(value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            if (typeof(TValue).IsEnum)
            {
                int enumInt = Unsafe.As<TValue, int>(ref value);
                m_Buffer->Add(enumInt);
                return VisitStatus.Handled;
            }

            return VisitStatus.Unhandled;
        }
    }


    unsafe public class BinaryPrimitiveReaderAdapter : IPropertyVisitorAdapter
        , IVisitAdapterPrimitives
        , IVisitAdapter<string>
        , IVisitAdapter
    {
        unsafe public BinaryPrimitiveReaderAdapter(UnsafeAppendBuffer.Reader* buffer)
        {
            Buffer = buffer;
        }

        public UnsafeAppendBuffer.Reader* Buffer;


        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref sbyte value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, sbyte>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref short value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, short>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref int value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, int>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref long value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, long>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref byte value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, byte>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref ushort value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, ushort>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref uint value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, uint>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref ulong value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, ulong>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref float value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, float>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref double value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, double>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref bool value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, bool>
        {
            byte byteValue;
            Buffer->ReadNext(out byteValue);
            value = byteValue != 0;
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref char value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, char>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        unsafe public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref string value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, string>
        {
            Buffer->ReadNext(out value);
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
            if (typeof(TValue).IsEnum)
            {
                int intEnum;
                Buffer->ReadNext(out intEnum);
                Unsafe.As<TValue, int>(ref value) = intEnum;
                return VisitStatus.Handled;
            }

            return VisitStatus.Unhandled;
        }
    }
}
#endif