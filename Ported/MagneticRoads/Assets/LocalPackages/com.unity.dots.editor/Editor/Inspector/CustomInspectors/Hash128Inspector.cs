using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor.Inspectors
{
    [UsedImplicitly]
    class Hash128Inspector : BaseFieldInspector<TextField, string, Hash128>
    {
        static Hash128Inspector()
        {
            TypeConversion.Register<Hash128, string>(v => v.ToString());
        }

        public override VisualElement Build()
        {
            var root = base.Build();
            m_Field.RegisterCallback<ChangeEvent<string>, TextField>(NoOp, m_Field);
            return root;
        }

        static void NoOp(ChangeEvent<string> evt, TextField field)
        {
            field.SetValueWithoutNotify(evt.previousValue);
        }
    }
}
