using Unity.Properties;
using Unity.Properties.UI;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class TagElement<TComponent> : ComponentElementBase
    {
        public TagElement(IComponentProperty property, EntityInspectorContext context) : base(property, context)
        {
            Resources.Templates.Inspector.TagComponentElement.Clone(this);
            this.Q<Label>().text = DisplayName;
            this.Q<VisualElement>(className: UssClasses.Inspector.Component.Icon).AddToClassList(UssClasses.Inspector.ComponentTypes.Tag);
        }

        protected override void OnComponentChanged(PropertyElement element, PropertyPath path)
        {
            // Nothing to do..
        }

        protected override void OnPopulateMenu(DropdownMenu menu)
        {
            // Nothing to do..
        }
    }
}
