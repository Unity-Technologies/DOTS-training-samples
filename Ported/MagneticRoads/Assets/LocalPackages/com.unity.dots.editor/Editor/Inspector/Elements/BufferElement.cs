using System.Collections.Generic;
using Unity.Properties;
using Unity.Properties.UI;
using Unity.Scenes;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    sealed class BufferElement<TList, TElement> : ComponentElementBase
        where TList : IList<TElement>
    {
        readonly PropertyElement m_Content;

        public BufferElement(IComponentProperty property, EntityInspectorContext context, ref InspectedBuffer<TList, TElement> value) : base(property, context)
        {
            m_Content = CreateContent(property, ref value);
        }

        protected override void OnComponentChanged(PropertyElement element, PropertyPath path)
        {
            var buffer = element.GetTarget<InspectedBuffer<TList, TElement>>();
            var container = Container;

            if (container.IsReadOnly)
                return;

            PropertyContainer.SetValue(ref container, Path, buffer.Value);
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }

        protected override void OnPopulateMenu(DropdownMenu menu)
        {
            var buffer = m_Content.GetTarget<InspectedBuffer<TList, TElement>>();
            menu.AddCopyValue(buffer.Value);
        }
    }
}
