using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    [UsedImplicitly]
    class GuidInspector : IInspector<GUID>
    {
        TextField m_Field;

        public VisualElement Build(InspectorContext<GUID> context)
        {
            m_Field = new TextField
            {
                label = context.PrettyName,
                tooltip = context.Tooltip
            };
            m_Field.SetValueWithoutNotify(context.Data.ToString());
            m_Field.SetEnabled(false);
            return m_Field;
        }

        public void Update(InspectorContext<GUID> context)
        {
            // Nothing to do..
        }
    }
}