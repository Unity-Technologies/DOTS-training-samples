using JetBrains.Annotations;
using Unity.Properties;

namespace Unity.Entities.Editor
{
    [DOTSEditorPreferencesSetting(Constants.Settings.Advanced), UsedImplicitly]
    class AdvancedSettings : ISetting
    {
        public bool ShowAdvancedWorlds;

        public void OnSettingChanged(PropertyPath path)
        {
        }
    }
}
