using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    abstract class MinDrawer<TElement, TFieldValue, TValue> : BaseFieldInspector<TElement, TFieldValue, TValue>, IPropertyDrawer<MinAttribute>
        where TElement : BaseField<TFieldValue>, new()
    {
        float m_MinValue;

        protected override Connector<TFieldValue, TValue> Connector { get; } = ConnectorFactory<TValue>.GetExact<BaseField<TFieldValue>, TFieldValue>();

        public override VisualElement Build(InspectorContext<TValue> context)
        {
            base.Build(context);
            m_MinValue = context.Attributes.GetAttribute<MinAttribute>().min;
            return m_Field;
        }

        protected override void OnChanged(ChangeEvent<TFieldValue> evt, InspectorContext<TValue> context)
        {
            context.Data = Connector.ToValue(
                TypeConversion.Convert<float, TFieldValue>(
                    Mathf.Max(m_MinValue,  TypeConversion.Convert<TFieldValue, float>(evt.newValue))));
        }
    }
    
    [UsedImplicitly] class MinSByteDrawer : MinDrawer<IntegerField, int, sbyte> { }
    [UsedImplicitly] class MinByteDrawer : MinDrawer<IntegerField, int, byte> { }
    [UsedImplicitly] class MinShortDrawer : MinDrawer<IntegerField, int, short> { }
    [UsedImplicitly] class MinUShortDrawer : MinDrawer<IntegerField, int, ushort> { }
    [UsedImplicitly] class MinIntDrawer : MinDrawer<IntegerField, int, int> { }
    [UsedImplicitly] class MinUIntDrawer : MinDrawer<LongField, long, uint> { }
    [UsedImplicitly] class MinLongDrawer : MinDrawer<LongField, long, long> { }
    [UsedImplicitly] class MinFloatDrawer : MinDrawer<FloatField, float, float> { }
    [UsedImplicitly] class DoubleFloatDrawer : MinDrawer<DoubleField, double, double> { }
}
