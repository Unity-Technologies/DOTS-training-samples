using System.Linq;
using JetBrains.Annotations;
using Unity.Properties.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Build
{
    [UsedImplicitly]
    sealed class AndroidSettingsInspector : IInspector<AndroidSettings>
    {
        PopupField<int> m_TargetApiPopup;
        
        public VisualElement Build(InspectorContext<AndroidSettings> context)
        {
            var root = new VisualElement();
            context.DoDefaultGui(root, nameof(AndroidSettings.PackageName));
            context.DoDefaultGui(root, nameof(AndroidSettings.TargetArchitectures));

            var minApiPopup = new PopupField<int>(
                ObjectNames.NicifyVariableName(nameof(AndroidSettings.MinAPILevel)),
                AndroidSettings.s_AndroidCodeNames.Keys.ToList(),
                0,
                value => AndroidSettings.s_AndroidCodeNames[value],
                value => AndroidSettings.s_AndroidCodeNames[value])
            {
                bindingPath = nameof(AndroidSettings.MinAPILevel)
            };
            root.contentContainer.Add(minApiPopup);

            m_TargetApiPopup = new PopupField<int>(
                ObjectNames.NicifyVariableName(nameof(AndroidSettings.TargetAPILevel)),
                AndroidSettings.s_AndroidCodeNames.Keys.ToList(),
                0,
                value => AndroidSettings.s_AndroidCodeNames[value],
                value => AndroidSettings.s_AndroidCodeNames[value])
            {
                bindingPath = nameof(AndroidSettings.TargetAPILevel)
            };
            root.contentContainer.Add(m_TargetApiPopup);
            
            return root;
        }

        public void Update(InspectorContext<AndroidSettings> context)
        {
            // Nothing to do.
        }
    }
}