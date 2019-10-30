using System.IO;
using Unity.Properties;
using Unity.Serialization.Json;

namespace Unity.Entities.Editor
{
    class LiveLinkSettings
    {
        static readonly string s_LiveLinkSettingsPath = "Library/LiveLinkSettings.json";

        static LiveLinkSettings s_Instance;

        [Property] string m_SelectedBuildSettingsAssetGuid;

        public string SelectedBuildSettingsAssetGuid
        {
            get => m_SelectedBuildSettingsAssetGuid;
            set
            {
                m_SelectedBuildSettingsAssetGuid = value;
                Save();
            }
        }

        public static LiveLinkSettings Instance { get; } = s_Instance ?? (s_Instance = CreateOrLoad());

        static LiveLinkSettings CreateOrLoad() => File.Exists(s_LiveLinkSettingsPath) ? JsonSerialization.DeserializeFromPath<LiveLinkSettings>(s_LiveLinkSettingsPath) : new LiveLinkSettings();

        void Save() => JsonSerialization.Serialize(s_LiveLinkSettingsPath, this);
    }
}