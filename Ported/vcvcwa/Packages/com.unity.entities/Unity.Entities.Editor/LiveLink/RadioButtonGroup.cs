using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class RadioButtonGroup : VisualElement, INotifyValueChanged<Toggle>
    {
        readonly UQueryState<Toggle> m_ToggleQuery;
        Toggle m_SelectedItem;

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<RadioButtonGroup, UxmlTraits> { }

        public RadioButtonGroup()
        {
            RegisterCallback<ChangeEvent<bool>>(OnToggleChanged);

            m_ToggleQuery = this.Query<Toggle>().Build();
        }

        void OnToggleChanged(ChangeEvent<bool> evt)
        {
            if (evt.target is Toggle t)
            {
                evt.StopPropagation();

                if (evt.newValue == false)
                {
                    t.SetValueWithoutNotify(true);
                    return;
                }

                var currentSelectedItem = default(Toggle);
                var newSelectedItem = default(Toggle);;
                var list = m_ToggleQuery.ToList();

                foreach (var toggle in list)
                {
                    if (currentSelectedItem != null && newSelectedItem != null)
                        break;

                    if (ReferenceEquals(t, toggle))
                        newSelectedItem = toggle;

                    if (toggle.value && !ReferenceEquals(t, toggle))
                    {
                        toggle.SetValueWithoutNotify(false);
                        currentSelectedItem = toggle;
                    }
                }

                using (var newEvent = ChangeEvent<Toggle>.GetPooled(currentSelectedItem, newSelectedItem))
                {
                    newEvent.target = this;
                    SendEvent(newEvent);
                }
            }
        }

        void INotifyValueChanged<Toggle>.SetValueWithoutNotify(Toggle newValue)
        {
            m_SelectedItem = newValue;
        }

        Toggle INotifyValueChanged<Toggle>.value
        {
            get => m_SelectedItem;
            set => m_SelectedItem = value;
        }
    }
}
