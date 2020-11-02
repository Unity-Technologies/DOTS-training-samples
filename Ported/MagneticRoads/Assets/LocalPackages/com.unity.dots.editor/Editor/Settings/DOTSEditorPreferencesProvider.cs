using System.Collections.Generic;
using UnityEditor;

namespace Unity.Entities.Editor
{
    sealed class DOTSEditorPreferencesProvider : Settings<DOTSEditorPreferencesSettingAttribute>
    {
        [SettingsProvider]
        public static SettingsProvider GetPreferences()
        {
            return HasAnySettings
                ? new DOTSEditorPreferencesProvider()
                : null;
        }

        DOTSEditorPreferencesProvider(IEnumerable<string> keywords = null) : base("DOTS/Editor", SettingsScope.User, keywords)
        {
        }
    }
}
