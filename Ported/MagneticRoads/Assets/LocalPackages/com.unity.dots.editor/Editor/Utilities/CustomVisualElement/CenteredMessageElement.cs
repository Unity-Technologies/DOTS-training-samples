using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class CenteredMessageElement : VisualElement
    {
        internal readonly Label m_Title;
        internal readonly Label m_Message;
        string m_TitleContent;
        string m_MessageContent;

        new class UxmlFactory : UxmlFactory<CenteredMessageElement, UxmlTraits> { }

        new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription { name = "title" };
            UxmlStringAttributeDescription m_Message = new UxmlStringAttributeDescription { name = "message" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((CenteredMessageElement)ve).Title = m_Title.GetValueFromBag(bag, cc);
                ((CenteredMessageElement)ve).Message = m_Message.GetValueFromBag(bag, cc);
            }
        }

        public CenteredMessageElement()
        {
            Resources.Templates.CenteredMessageElement.Clone(this);
            style.flexGrow = 1;

            m_Title = this.Q<Label>(className: UssClasses.DotsEditorCommon.CenteredMessageElementTitle);
            m_Message = this.Q<Label>(className: UssClasses.DotsEditorCommon.CenteredMessageElementMessage);
        }

        public string Title
        {
            get => m_TitleContent;
            set
            {
                if (m_TitleContent == value)
                    return;

                m_TitleContent = value;
                m_Title.ToggleVisibility(!string.IsNullOrWhiteSpace(m_TitleContent));
                m_Title.text = m_TitleContent;
            }
        }

        public string Message
        {
            get => m_MessageContent;
            set
            {
                if (m_MessageContent == value)
                    return;

                m_MessageContent = value;
                m_Message.ToggleVisibility(!string.IsNullOrWhiteSpace(m_MessageContent));
                m_Message.text = m_MessageContent;
            }
        }
    }
}
