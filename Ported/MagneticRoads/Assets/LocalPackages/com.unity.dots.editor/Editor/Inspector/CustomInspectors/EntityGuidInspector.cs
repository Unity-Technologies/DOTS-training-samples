using JetBrains.Annotations;
using Unity.Properties.UI;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor.Inspectors
{
    [UsedImplicitly]
    class EntityGuidInspector : Inspector<EntityGuid>
    {
        static readonly string k_OriginatingIdName =
            ObjectNames.NicifyVariableName(nameof(EntityGuid.OriginatingId));

        static readonly string k_SerialName = ObjectNames.NicifyVariableName(nameof(EntityGuid.Serial));

        public override VisualElement Build()
        {
            var root = new VisualElement();
            var id = new TextField(k_OriginatingIdName) { value = Target.OriginatingId.ToString() };
            id.RegisterCallback<ChangeEvent<string>, TextField>(NoOp, id);
            root.Add(id);
            var serialId = new TextField(k_SerialName) { value = Target.Serial.ToString() };
            serialId.RegisterCallback<ChangeEvent<string>, TextField>(NoOp, serialId);
            root.Add(serialId);
            return root;
        }

        static void NoOp(ChangeEvent<string> evt, TextField field)
        {
            field.SetValueWithoutNotify(evt.previousValue);
        }
    }
}
