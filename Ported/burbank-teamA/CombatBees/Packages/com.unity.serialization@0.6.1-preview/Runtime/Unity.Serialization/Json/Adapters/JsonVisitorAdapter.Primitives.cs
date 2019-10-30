using System;
using System.Globalization;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    class JsonVisitorAdapterPrimitives : JsonVisitorAdapter
        , IVisitAdapterPrimitives
        , IVisitAdapter<string>
        , IVisitAdapter
    {
        public JsonVisitorAdapterPrimitives(JsonVisitor visitor) : base(visitor) { }

        public static void RegisterTypes()
        {
            TypeConversion.Register<SerializedStringView, string>(v => v.ToString());
            TypeConversion.Register<SerializedStringView, char>(v => v[0]);
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref sbyte value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, sbyte>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref short value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, short>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref int value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, int>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref long value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, long>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref byte value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, byte>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref ushort value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, ushort>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref uint value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, uint>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref ulong value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, ulong>
        {
            Append(property, value, (builder, v) => { builder.Append(v); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref float value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, float>
        {
            Append(property, value, (builder, v) => { builder.Append(v.ToString(CultureInfo.InvariantCulture)); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref double value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, double>
        {
            Append(property, value, (builder, v) => { builder.Append(v.ToString(CultureInfo.InvariantCulture)); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref bool value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, bool>
        {
            Append(property, value, (builder, v) => { builder.Append(v ? "true" : "false"); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref char value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, char>
        {
            Append(property, value, (builder, v) => { builder.Append(EncodeJsonString(string.Empty + v)); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref string value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, string>
        {
            Append(property, value, (builder, v) => { builder.Append(EncodeJsonString(v)); });
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker) where TProperty : IProperty<TContainer, TValue>
        {
            if (!typeof(TValue).IsEnum)
                return VisitStatus.Unhandled;

            Append(property, value, (builder, v) =>
            {
                var underlyingType = Enum.GetUnderlyingType(typeof(TValue));
                switch (Type.GetTypeCode(underlyingType))
                {
                    case TypeCode.Byte:
                        builder.Append((byte)(object)v);
                        break;
                    case TypeCode.Int16:
                        builder.Append((short)(object)v);
                        break;
                    case TypeCode.Int32:
                        builder.Append((int)(object)v);
                        break;
                    case TypeCode.Int64:
                        builder.Append((long)(object)v);
                        break;
                    case TypeCode.SByte:
                        builder.Append((sbyte)(object)v);
                        break;
                    case TypeCode.UInt16:
                        builder.Append((ushort)(object)v);
                        break;
                    case TypeCode.UInt32:
                        builder.Append((uint)(object)v);
                        break;
                    case TypeCode.UInt64:
                        builder.Append((ulong)(object)v);
                        break;
                    default:
                        throw new InvalidOperationException($"Unable to serialize enum value: {v} of type {typeof(TValue).FullName}.");
                }
            });
            return VisitStatus.Handled;
        }
    }
}
