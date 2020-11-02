using System;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Base attribute to tag a <see cref="ISetting"/> derived class as a setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    abstract class SettingsAttribute : Attribute
    {
        /// <summary>
        /// The name of the section to use for the setting.
        /// </summary>
        public string SectionName { get; }

        /// <summary>
        /// Constructs a new instance of <see cref="SettingsAttribute"/> with the provided section name.
        /// </summary>
        /// <param name="sectionName"></param>
        protected SettingsAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }
}
