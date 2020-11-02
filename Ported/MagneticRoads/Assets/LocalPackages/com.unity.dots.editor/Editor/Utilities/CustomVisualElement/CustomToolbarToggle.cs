using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Unity.Entities.Editor
{
    class CustomToolbarToggle : ToolbarToggle
    {
        Label m_Label;

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public CustomToolbarToggle()
        {
            Resources.Templates.DotsEditorCommon.AddStyles(this);

            this.text = "temp";
            m_Label = this.Q<Label>();
            m_Label.AddToClassList(UssClasses.DotsEditorCommon.CustomToolbarToggleOnlyLabel);

            this.AddToClassList(UssClasses.DotsEditorCommon.CustomToolbarToggle);
            m_Label.parent.AddToClassList(UssClasses.DotsEditorCommon.CustomToolbarToggleLabelParent);
        }

        public void AddPreIcon(Image preIcon)
        {
            this.Insert(0, preIcon);
            preIcon.RegisterCallback<MouseDownEvent>(evt =>
            {
                this.value = !this.value;
            });
            preIcon.AddToClassList(UssClasses.DotsEditorCommon.CustomToolbarToggleIcon);

            m_Label = this.Q<Label>();

            m_Label.RemoveFromClassList(UssClasses.DotsEditorCommon.CustomToolbarToggleOnlyLabel);
            m_Label.AddToClassList(UssClasses.DotsEditorCommon.CustomToolbarToggleLabel);
        }
    }
}
