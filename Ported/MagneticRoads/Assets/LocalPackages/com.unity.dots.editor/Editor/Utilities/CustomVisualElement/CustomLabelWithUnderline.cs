using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class CustomLabelWithUnderline : VisualElement
    {
        public string text
        {
            get => m_Label.text;
            set => m_Label.text = value;
        }

        Label m_Label;

        /// <summary>
        ///  <para>Constructor.</para>
        /// </summary>
        public CustomLabelWithUnderline()
        {
            m_Label = new Label();
            Resources.Templates.DotsEditorCommon.AddStyles(m_Label);
            m_Label.AddToClassList(UssClasses.DotsEditorCommon.CustomLabelUnderline);
            this.Add(m_Label);
        }
    }
}
