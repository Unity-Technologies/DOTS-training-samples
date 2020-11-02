using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    static class UIElementHelper
    {
        /// <summary>
        /// <para>Show this visual element.</para>
        /// </summary>
        public static void Show(VisualElement v) => ToggleVisibility(v, true);

        /// <summary>
        /// <para>Hide this visual element.</para>
        /// </summary>
        public static void Hide(VisualElement v) => ToggleVisibility(v, false);

        /// <summary>
        /// <para>Toggle visibility of this visual element.</para>
        /// </summary>
        public static void ToggleVisibility(VisualElement v, bool isVisible) => v.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;

        /// <summary>
        /// <para>Return if this visual element is visible or not.</para>
        /// </summary>
        public static bool IsVisible(VisualElement v) => v.style.display == DisplayStyle.Flex;
    }
}
