using Unity.Properties;
using Unity.Properties.UI;
using Unity.Scenes;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    sealed class ComponentElement<TComponent> : ComponentElementBase, IBinding
    {
        readonly PropertyElement m_Content;

        public ComponentElement(IComponentProperty property, EntityInspectorContext context, ref TComponent value)
            : base(property, context)
        {
            binding = this;
            m_Content = CreateContent(property, ref value);
        }

        protected override void OnComponentChanged(PropertyElement element, PropertyPath path)
        {
            var component = element.GetTarget<TComponent>();
            var container = Container;

            if (container.IsReadOnly)
                return;

            PropertyContainer.SetValue(ref container, Path, component);
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }

        protected override void OnPopulateMenu(DropdownMenu menu)
        {
            var container = Container;
            menu.AddCopyValue(PropertyContainer.GetValue<EntityContainer, TComponent>(ref container, Path));
        }

        void IBinding.PreUpdate()
        {
        }

        void IBinding.Update()
        {
            var container = Container;
            if (!Context.World.IsCreated || !Context.EntityManager.Exists(Container.Entity))
            {
                RemoveFromHierarchy();
                return;
            }

            if (PropertyContainer.TryGetValue<EntityContainer, TComponent>(ref container, Path, out var component))
                m_Content.SetTarget(component);
        }

        void IBinding.Release()
        {
        }
    }
}
