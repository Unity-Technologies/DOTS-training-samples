namespace Unity.Entities.Editor
{
    /// <summary>
    /// Attribute to tag a type to be included as a setting for the DOTS Editor project settings.
    /// </summary>
    sealed class DOTSEditorProjectSettingsAttribute : SettingsAttribute
    {
        /// <summary>
        /// Constructs a new instance of <see cref="DOTSEditorProjectSettingsAttribute"/> with the provided section name.
        /// </summary>
        /// <param name="sectionName">The name of the section where the setting should go.</param>
        public DOTSEditorProjectSettingsAttribute(string sectionName) : base(sectionName)
        {
        }
    }
}
