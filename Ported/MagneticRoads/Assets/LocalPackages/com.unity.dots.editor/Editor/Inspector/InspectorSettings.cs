using JetBrains.Annotations;
using Unity.Editor.Bridge;
using Unity.Properties;

namespace Unity.Entities.Editor
{
    [DOTSEditorPreferencesSetting(Constants.Settings.Inspector), UsedImplicitly]
    class InspectorSettings : ISetting
    {
        public enum InspectorBackend
        {
            [UsedImplicitly] Debug = 0,
            [UsedImplicitly] Normal = 1
        }

        [InternalSetting]
        public InspectorBackend Backend = InspectorBackend.Normal;

        [InternalSetting]
        public bool DisplayComponentType = false;

        void ISetting.OnSettingChanged(PropertyPath path)
        {
            var p = path.ToString();
            if (p == nameof(Backend))
                InspectorWindowBridge.ReloadAllInspectors();
        }
    }
}
