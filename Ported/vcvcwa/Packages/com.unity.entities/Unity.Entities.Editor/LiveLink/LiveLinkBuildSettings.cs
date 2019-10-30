using System;
using Unity.Build;
using UnityEditor;

namespace Unity.Entities.Editor
{
    static class LiveLinkBuildSettings
    {
        static BuildSettings s_CurrentLiveLinkBuildSettings;

        internal static event Action CurrentLiveLinkBuildSettingsChanged = delegate { };

        public static BuildSettings CurrentLiveLinkBuildSettings
        {
            get
            {
                if (s_CurrentLiveLinkBuildSettings != null)
                    return s_CurrentLiveLinkBuildSettings;

                var selectedGuid = LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid;
                if (string.IsNullOrEmpty(selectedGuid))
                    return null;

                return s_CurrentLiveLinkBuildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>(AssetDatabase.GUIDToAssetPath(selectedGuid));
            }
            set
            {
                if (value == CurrentLiveLinkBuildSettings)
                    return;

                s_CurrentLiveLinkBuildSettings = value;

                LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid = s_CurrentLiveLinkBuildSettings != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(s_CurrentLiveLinkBuildSettings)) : string.Empty;
                CurrentLiveLinkBuildSettingsChanged();
            }
        }
    }
}