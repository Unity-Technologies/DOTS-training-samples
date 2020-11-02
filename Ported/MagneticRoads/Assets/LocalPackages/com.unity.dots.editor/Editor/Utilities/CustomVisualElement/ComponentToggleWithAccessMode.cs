using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Unity.Entities.Editor
{
    class ComponentToggleWithAccessMode : VisualElement
    {
        public readonly CustomToolbarToggle ComponentTypeNameToggle;

        /// <summary>
        /// Constructor of the ComponentTypeVisualElement.
        /// Given an component type, a visual element contains <see cref="CustomToolbarToggle"/>
        /// and an icon representing its access mode will be created.
        /// <para><see cref="ComponentType"/>></para>>
        /// </summary>
        public ComponentToggleWithAccessMode(ComponentType.AccessMode accessMode)
        {
            this.AddToClassList(UssClasses.SystemScheduleWindow.Detail.EachComponentContainer);

            // Access mode.
            var componentAccessModeIcon = new Image()
            {
                tooltip = accessMode.ToString()
            };
            componentAccessModeIcon.AddToClassList(UssClasses.SystemScheduleWindow.Detail.ComponentAccessModeIcon);
            componentAccessModeIcon.AddToClassList(EntityQueryUtility.StyleForAccessMode(accessMode));

            // Component toggle.
            ComponentTypeNameToggle = new CustomToolbarToggle();
            ComponentTypeNameToggle.AddPreIcon(componentAccessModeIcon);
            this.Add(ComponentTypeNameToggle);
        }
    }
}
