using JetBrains.Annotations;
using Unity.Properties.UI;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor.Inspectors
{
    [UsedImplicitly]
    class EntityInspector : Inspector<Entity>
    {
        EntityField m_Field;

        World GetWorld()
        {
            var context = GetContext<EntityInspectorContext>();
            return context?.World;
        }

        public override VisualElement Build()
        {
            m_Field = new EntityField(DisplayName) { value = Target, World = GetWorld() };
            return m_Field;
        }

        public override void Update()
        {
            m_Field.World = GetWorld();
            m_Field.SetValueWithoutNotify(Target);
        }
    }
}
