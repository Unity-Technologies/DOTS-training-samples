using System.Collections.Generic;
using UnityEditor;

namespace Unity.Entities.Editor
{
    sealed class DOTSEditorProjectSettingsProvider : Settings<DOTSEditorProjectSettingsAttribute>
    {
        [SettingsProvider]
        public static SettingsProvider GetPreferences()
        {
            return HasAnySettings
                ? new DOTSEditorProjectSettingsProvider()
                : null;
        }

        protected override string Title { get; } = "Editor Settings for DOTS";

        DOTSEditorProjectSettingsProvider(IEnumerable<string> keywords = null) : base("DOTS/Editor", SettingsScope.Project, keywords)
        {
        }
    }
}
