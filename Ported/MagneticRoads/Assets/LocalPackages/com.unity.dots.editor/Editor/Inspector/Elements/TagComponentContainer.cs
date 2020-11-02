using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    sealed class TagComponentContainer : BindableElement, IBinding
    {
        public override VisualElement contentContainer { get; }
        EntityInspectorContext Context { get; }

        public TagComponentContainer(EntityInspectorContext context)
        {
            Context = context;
            Resources.Templates.CommonResources.AddStyles(this);
            AddToClassList(UssClasses.Resources.Inspector);
            AddToClassList(UssClasses.Resources.ComponentIcons);

            InspectorUtility.CreateComponentHeader(this, ComponentPropertyType.Tag, "Tags");
            var foldout = this.Q<Foldout>(className: UssClasses.Inspector.Component.Header);
            contentContainer = foldout.contentContainer;
            binding = this;
            binding.Update();

            contentContainer.SetEnabled(!Context.IsReadOnly);
        }

        public void PreUpdate()
        {
        }

        public void Update()
        {
            // The tag container will only be shown if there are actual tags that are displayed.
            if (contentContainer.Children().Any(e => e.style.display == DisplayStyle.Flex || e.style.display == new StyleEnum<DisplayStyle>(StyleKeyword.Null)))
                this.Show();
            else
                this.Hide();
        }

        public void Release()
        {
        }
    }
}
