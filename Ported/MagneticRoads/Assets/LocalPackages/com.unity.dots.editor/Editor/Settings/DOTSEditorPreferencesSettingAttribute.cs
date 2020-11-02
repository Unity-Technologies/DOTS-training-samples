namespace Unity.Entities.Editor
{
    /// <summary>
    /// Attribute to tag a type to be included as a setting for the DOTS Editor preferences.
    /// </summary>
    sealed class DOTSEditorPreferencesSettingAttribute : SettingsAttribute
    {
        /// <summary>
        /// Constructs a new instance of <see cref="DOTSEditorPreferencesSettingAttribute"/> with the provided section name.
        /// </summary>
        /// <param name="sectionName">The name of the section where the setting should go.</param>
        public DOTSEditorPreferencesSettingAttribute(string sectionName) : base(sectionName)
        {
        }
    }
}
