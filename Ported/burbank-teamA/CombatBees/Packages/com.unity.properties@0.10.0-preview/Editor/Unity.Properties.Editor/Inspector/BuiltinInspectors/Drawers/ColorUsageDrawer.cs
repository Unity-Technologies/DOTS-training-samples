using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    [UsedImplicitly]
    class ColorUsageDrawer : ColorInspector, IPropertyDrawer<ColorUsageAttribute>
    {
        public override VisualElement Build(InspectorContext<Color> context)
        {
            var element = base.Build(context);
            var usage = context.Attributes.GetAttribute<ColorUsageAttribute>();
            m_Field.hdr = usage.hdr;
            m_Field.showAlpha = usage.showAlpha;
            return element;
        }
    }
}