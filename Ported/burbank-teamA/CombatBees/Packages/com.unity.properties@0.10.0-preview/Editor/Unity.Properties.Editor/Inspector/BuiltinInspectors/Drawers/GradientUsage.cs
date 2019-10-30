using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    [UsedImplicitly]
    class GradientUsageDrawer : GradientInspector, IPropertyDrawer<GradientUsageAttribute>
    {
        public override VisualElement Build(InspectorContext<Gradient> context)
        {
            var element = base.Build(context);
            // GradientField.hdr is not yet supported.
            //var usage = context.Attributes.GetAttribute<GradientUsageAttribute>();
            //m_Field.hdr = usage.hdr;
            return element;
        }
    }
}