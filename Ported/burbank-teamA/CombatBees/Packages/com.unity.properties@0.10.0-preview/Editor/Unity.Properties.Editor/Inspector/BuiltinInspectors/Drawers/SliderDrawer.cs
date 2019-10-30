using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    abstract class SliderInspectorBase<TSlider, TFieldType, TValue> : BaseFieldInspector<TSlider, TFieldType, TValue>, IPropertyDrawer<RangeAttribute>
        where TSlider : BaseSlider<TFieldType>, new()
        where TFieldType : IComparable<TFieldType>
    {
        protected override Connector<TFieldType, TValue> Connector { get; } = ConnectorFactory<TValue>.GetExact<BaseField<TFieldType>, TFieldType>();
        protected abstract float LowValue { get; }
        protected abstract float HighValue { get; }

        public override VisualElement Build(InspectorContext<TValue> context)
        {
            base.Build(context);
            var range = context.Attributes.GetAttribute<RangeAttribute>();
            m_Field.lowValue = TypeConversion.Convert<float, TFieldType>(Mathf.Max(range.min, LowValue));
            m_Field.highValue = TypeConversion.Convert<float, TFieldType>(Mathf.Min(range.max, HighValue));
            return m_Field;
        }
    }

    [UsedImplicitly]
    sealed class SByteSliderInspectorBase : SliderInspectorBase<SliderInt, int, sbyte>
    {
        protected override float LowValue { get; } = sbyte.MinValue;
        protected override float HighValue { get; } = sbyte.MaxValue;
    }
    
    [UsedImplicitly]
    sealed class ByteSliderInspectorBase : SliderInspectorBase<SliderInt, int, byte>
    {
        protected override float LowValue { get; } = byte.MinValue;
        protected override float HighValue { get; } = byte.MaxValue;
    }
    
    [UsedImplicitly] 
    sealed class ShortSliderInspectorBase : SliderInspectorBase<SliderInt, int, short>
    {
        protected override float LowValue { get; } = short.MinValue;
        protected override float HighValue { get; } = short.MaxValue;
    }

    [UsedImplicitly]
    sealed class UShortSliderInspectorBase : SliderInspectorBase<SliderInt, int, ushort>
    {
        protected override float LowValue { get; } = ushort.MinValue;
        protected override float HighValue { get; } = ushort.MaxValue;
    }

    [UsedImplicitly]
    sealed class SliderInspectorBase : SliderInspectorBase<SliderInt, int, int>
    {
        protected override float LowValue { get; } = int.MinValue;
        protected override float HighValue { get; } = int.MaxValue;
    }
    
    [UsedImplicitly]
    class SliderInspector : SliderInspectorBase<Slider, float, float>
    {
        protected override Connector<float, float> Connector { get; } = ConnectorFactory<float>.GetExact<BaseField<float>, float>();
        protected override float LowValue { get; } = float.MinValue;
        protected override float HighValue { get; } = float.MaxValue;
    }
}
