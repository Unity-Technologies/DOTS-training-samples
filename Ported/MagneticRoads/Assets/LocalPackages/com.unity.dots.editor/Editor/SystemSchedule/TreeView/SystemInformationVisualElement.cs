using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class SystemInformationVisualElement : BindableElement, IBinding, IPoolable
    {
        public World World;
        SystemTreeViewItem m_Target;
        public SystemScheduleTreeView TreeView { get; set; }
        const float k_SystemNameLabelWidth = 100f;
        const float k_SingleIndentWidth = 12f;

        public SystemTreeViewItem Target
        {
            get => m_Target;
            set
            {
                if (m_Target == value)
                    return;
                m_Target = value;
                Update();
            }
        }

        readonly Toggle m_SystemEnableToggle;
        readonly VisualElement m_Icon;
        readonly Label m_SystemNameLabel;
        readonly Label m_EntityMatchLabel;
        readonly Label m_RunningTimeLabel;

        public SystemInformationVisualElement()
        {
            Resources.Templates.CommonResources.AddStyles(this);
            Resources.Templates.SystemScheduleItem.Clone(this);
            binding = this;

            AddToClassList(UssClasses.DotsEditorCommon.CommonResources);
            AddToClassList(UssClasses.Resources.SystemSchedule);

            m_SystemEnableToggle = this.Q<Toggle>(className: UssClasses.SystemScheduleWindow.Items.Enabled);
            m_SystemEnableToggle.RegisterCallback<ChangeEvent<bool>, SystemInformationVisualElement>(
                OnSystemTogglePress, this);

            m_Icon = this.Q(className: UssClasses.SystemScheduleWindow.Items.Icon);

            m_SystemNameLabel = this.Q<Label>(className: UssClasses.SystemScheduleWindow.Items.SystemName);
            m_EntityMatchLabel = this.Q<Label>(className: UssClasses.SystemScheduleWindow.Items.Matches);
            m_RunningTimeLabel = this.Q<Label>(className: UssClasses.SystemScheduleWindow.Items.Time);
        }

        static void SetText(Label label, string text)
        {
            if (label.text != text)
                label.text = text;
        }

        public unsafe void Update()
        {
            if (null == Target)
                return;

            if (Target.selectedSystem.Valid && Target.selectedSystem.World == null)
                return;

            var ptr = Target.selectedSystem.StatePointer;
            m_Icon.style.display = string.Empty == GetSystemClass(Target.selectedSystem) ? DisplayStyle.None : DisplayStyle.Flex;

            SetText(m_SystemNameLabel, Target.GetSystemName(World));
            SetSystemNameLabelWidth(m_SystemNameLabel, k_SystemNameLabelWidth);
            SetText(m_EntityMatchLabel, Target.GetEntityMatches());
            SetText(m_RunningTimeLabel, Target.GetRunningTime());
            SetSystemClass(m_Icon, Target.selectedSystem);
            SetGroupNodeLabelBold(m_SystemNameLabel, Target.selectedSystem);

            if (Target.selectedSystem == null) // player loop system without children
            {
                SetEnabled(Target.HasChildren);
                m_SystemEnableToggle.style.display = DisplayStyle.None;
            }
            else
            {
                SetEnabled(true);
                m_SystemEnableToggle.style.display = DisplayStyle.Flex;
                var systemState = ptr == null || ptr->Enabled;

                if (m_SystemEnableToggle.value != systemState)
                    m_SystemEnableToggle.SetValueWithoutNotify(systemState);

                var groupState = systemState && Target.GetParentState();

                m_SystemNameLabel.SetEnabled(groupState);
                m_EntityMatchLabel.SetEnabled(groupState);
                m_RunningTimeLabel.SetEnabled(groupState);
            }
        }

        void SetSystemNameLabelWidth(VisualElement label, float fixedWidth)
        {
            var treeViewItemVisualElement = parent.parent;
            var itemIndentsContainerName = treeViewItemVisualElement.Q("unity-tree-view__item-indents");
            label.style.width = fixedWidth - itemIndentsContainerName.childCount * k_SingleIndentWidth;
        }

        static void SetSystemClass(VisualElement element, SelectedSystem selectedSystem)
        {
            if (selectedSystem == null)
            {
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.CommandBufferIcon);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemGroupIcon);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemIcon);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.UnmanagedSystemIcon);
                return;
            }

            if (selectedSystem.Managed != null)
            {
                switch (selectedSystem.Managed)
                {
                    case EntityCommandBufferSystem _:
                        element.AddToClassList(UssClasses.SystemScheduleWindow.Items.CommandBufferIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemGroupIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.UnmanagedSystemIcon);
                        break;
                    case ComponentSystemGroup _:
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.CommandBufferIcon);
                        element.AddToClassList(UssClasses.SystemScheduleWindow.Items.SystemGroupIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.UnmanagedSystemIcon);
                        break;
                    case ComponentSystemBase _:
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.CommandBufferIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemGroupIcon);
                        element.AddToClassList(UssClasses.SystemScheduleWindow.Items.SystemIcon);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.UnmanagedSystemIcon);
                        break;
                }
            }
            else if (selectedSystem.Unmanaged.World != null)
            {
                element.AddToClassList(UssClasses.SystemScheduleWindow.Items.UnmanagedSystemIcon);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.CommandBufferIcon);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemGroupIcon);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemIcon);
            }
        }

        static void SetGroupNodeLabelBold(VisualElement element, SelectedSystem selectedSystem)
        {
            if (selectedSystem == null)
            {
                element.AddToClassList(UssClasses.SystemScheduleWindow.Items.SystemNameBold);
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemNameNormal);
                return;
            }

            if (selectedSystem.Managed != null)
            {
                switch (selectedSystem.Managed)
                {
                    case ComponentSystemGroup _:
                        element.AddToClassList(UssClasses.SystemScheduleWindow.Items.SystemNameBold);
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemNameNormal);
                        break;
                    case EntityCommandBufferSystem _:
                    case ComponentSystemBase _:
                        element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemNameBold);
                        element.AddToClassList(UssClasses.SystemScheduleWindow.Items.SystemNameNormal);
                        break;
                }
            }
            else if (selectedSystem.Unmanaged.World != null)
            {
                element.RemoveFromClassList(UssClasses.SystemScheduleWindow.Items.SystemNameBold);
                element.AddToClassList(UssClasses.SystemScheduleWindow.Items.SystemNameNormal);
            }
        }

        static string GetSystemClass(SelectedSystem selectedSystem)
        {
            if (selectedSystem == null)
                return string.Empty;

            if (selectedSystem.Managed != null)
            {
                switch (selectedSystem.Managed)
                {
                    case EntityCommandBufferSystem _:
                        return UssClasses.SystemScheduleWindow.Items.CommandBufferIcon;
                    case ComponentSystemGroup _:
                        return UssClasses.SystemScheduleWindow.Items.SystemGroupIcon;
                    case ComponentSystemBase _:
                        return UssClasses.SystemScheduleWindow.Items.SystemIcon;
                }
            }
            else if (selectedSystem.Unmanaged.World != null)
            {
                return UssClasses.SystemScheduleWindow.Items.UnmanagedSystemIcon;
            }

            return string.Empty;
        }

        static void OnSystemTogglePress(ChangeEvent<bool> evt, SystemInformationVisualElement item)
        {
            if (item.Target.selectedSystem != null)
            {
                item.Target.SetSystemState(evt.newValue);
            }
            else
            {
                item.Target.SetPlayerLoopSystemState(evt.newValue);
            }
        }

        public void PreUpdate()
        {
        }

        public void Release()
        {
        }

        public void Reset()
        {
            World = null;
            Target = null;
            TreeView = null;
        }

        public void ReturnToPool()
        {
            SystemSchedulePool.ReturnToPool(TreeView, this);
        }
    }
}
