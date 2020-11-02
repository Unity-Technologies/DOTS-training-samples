using Unity.Properties.UI;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor.Inspectors
{
    abstract class BaseFieldInspector<TField, TFieldValue, TValue> : Inspector<TValue>
        where TField : BaseField<TFieldValue>, new()
    {
        protected TField m_Field;

        public override VisualElement Build()
        {
            m_Field = new TField
            {
                name = Name,
                label = DisplayName,
                tooltip = Tooltip,
                bindingPath = "."
            };
            return m_Field;
        }
    }
}
