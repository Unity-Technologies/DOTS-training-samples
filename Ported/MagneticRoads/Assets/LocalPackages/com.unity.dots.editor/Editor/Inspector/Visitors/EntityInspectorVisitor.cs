using Unity.Properties;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class EntityInspectorVisitor : PropertyVisitor
    {
        readonly VisualElement m_ComponentRoot;
        readonly VisualElement m_TagsComponentRoot;
        readonly EntityInspectorContext m_Context;

        public EntityInspectorVisitor(VisualElement componentRoot, VisualElement tagRoot, EntityInspectorContext context)
        {
            m_ComponentRoot = componentRoot;
            m_TagsComponentRoot = tagRoot;
            m_Context = context;
        }

        protected override void VisitProperty<TContainer, TValue>(Property<TContainer, TValue> property,
            ref TContainer container, ref TValue value)
        {
            if (!(property is IComponentProperty componentProperty))
                return;

            if (componentProperty.Type == ComponentPropertyType.Tag)
            {
                var element = new TagElement<TValue>(componentProperty, m_Context);
                m_TagsComponentRoot.Add(element);
            }
            else
            {
                var element = new ComponentElement<TValue>(componentProperty, m_Context, ref value);
                m_ComponentRoot.Add(element);
            }
        }

        protected override void VisitList<TContainer, TList, TElement>(Property<TContainer, TList> property,
            ref TContainer container, ref TList value)
        {
            if (!(property is IComponentProperty componentProperty) || componentProperty.Type != ComponentPropertyType.Buffer)
                return;

            var buffer = new InspectedBuffer<TList, TElement> { Value = value };
            var element = new BufferElement<TList, TElement>(componentProperty, m_Context, ref buffer);
            m_ComponentRoot.Add(element);
        }
    }
}
